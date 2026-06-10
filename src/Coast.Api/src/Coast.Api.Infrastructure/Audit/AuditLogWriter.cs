using Coast.Api.Infrastructure.DataPersistence.EfCore;
using Coast.Api.Infrastructure.DataPersistence.EfCore.Entities;
using Coast.Api.Infrastructure.DataPersistence.EfCore.Entities.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Coast.Api.Infrastructure.Audit;

/// <summary>
/// 审计日志写入器接口
/// </summary>
public interface IAuditLogWriter
{
    /// <summary>批量写入审计日志</summary>
    Task WriteAsync(List<AuditEntry> entries);

    /// <summary>写入单条审计日志</summary>
    Task WriteAsync(AuditEntry entry);

    /// <summary>写入操作日志（无字段变更）</summary>
    Task WriteOperationLogAsync(Guid? userId, string? userName, OperationType operationType,
        TargetType targetType, string targetId, string targetName,
        string? description = null, string? ipAddress = null, string? userAgent = null, Guid? sessionId = null);
}

/// <summary>
/// 审计日志写入器实现
/// </summary>
public class AuditLogWriter : IAuditLogWriter
{
    private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
    private readonly ILogger<AuditLogWriter> _logger;

    public AuditLogWriter(
        IDbContextFactory<ApplicationDbContext> contextFactory,
        ILogger<AuditLogWriter> logger)
    {
        _contextFactory = contextFactory;
        _logger = logger;
    }

    public async Task WriteAsync(List<AuditEntry> entries)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        foreach (var entry in entries)
        {
            await WriteEntryInternalAsync(context, entry);
        }
    }

    public async Task WriteAsync(AuditEntry entry)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        await WriteEntryInternalAsync(context, entry);
    }

    public async Task WriteOperationLogAsync(Guid? userId, string? userName, OperationType operationType,
        TargetType targetType, string targetId, string targetName,
        string? description = null, string? ipAddress = null, string? userAgent = null, Guid? sessionId = null)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        var operationLog = new OperationLog
        {
            OperatorId = userId,
            OperatorName = userName,
            OperationType = operationType,
            TargetType = targetType,
            TargetId = targetId,
            TargetName = targetName,
            Description = description,
            IpAddress = ipAddress,
            UserAgent = userAgent,
            SessionId = sessionId,
            CreatedOn = DateTime.UtcNow
        };

        await context.Set<OperationLog>().AddAsync(operationLog);
        await context.SaveChangesAsync();
    }

    private async Task WriteEntryInternalAsync(ApplicationDbContext context, AuditEntry entry)
    {
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            // 创建操作日志
            var operationLog = new OperationLog
            {
                OperatorId = entry.AuditContext?.UserId,
                OperatorName = entry.AuditContext?.UserName,
                OperationType = entry.OperationType,
                TargetType = entry.EntityType,
                TargetId = entry.EntityId,
                TargetName = entry.EntityName,
                IpAddress = entry.AuditContext?.IpAddress,
                UserAgent = entry.AuditContext?.UserAgent,
                Description = BuildDescription(entry),
                ExtraData = BuildExtraData(entry),
                SessionId = entry.AuditContext?.SessionId,
                CreatedOn = entry.OperationTime
            };

            await context.Set<OperationLog>().AddAsync(operationLog);
            await context.SaveChangesAsync();

            // 创建字段变更日志（仅针对 Update 操作）
            if (entry.OperationType == OperationType.Update && entry.ChangedProperties.Any())
            {
                foreach (var change in entry.ChangedProperties)
                {
                    var dataChangeLog = new DataChangeLog
                    {
                        OperationLogId = operationLog.Id,
                        EntityType = entry.EntityType,
                        EntityId = entry.EntityId,
                        FieldName = change.PropertyName,
                        FieldDisplayName = change.PropertyDisplayName,
                        FieldType = change.FieldType,
                        OldValue = change.IsSensitive ? "[REDACTED]" : SerializeValue(change.OldValue),
                        NewValue = change.IsSensitive ? "[REDACTED]" : SerializeValue(change.NewValue),
                        CreatedOn = entry.OperationTime
                    };

                    await context.Set<DataChangeLog>().AddAsync(dataChangeLog);
                }

                await context.SaveChangesAsync();
            }

            await transaction.CommitAsync();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "写入审计日志失败: {EntityType} {EntityId}",
                entry.EntityType, entry.EntityId);
        }
    }

    private static string BuildDescription(AuditEntry entry)
    {
        return entry.OperationType switch
        {
            OperationType.Create => $"创建 {entry.EntityName}",
            OperationType.Update => $"更新 {entry.EntityName}，变更 {entry.ChangedProperties.Count} 个字段",
            OperationType.Delete => $"删除 {entry.EntityName}",
            _ => $"{entry.OperationType} {entry.EntityName}"
        };
    }

    private static string? BuildExtraData(AuditEntry entry)
    {
        if (!entry.ChangedProperties.Any()) return null;

        var extra = new Dictionary<string, object?>
        {
            ["EntityId"] = entry.EntityId,
            ["ChangedFields"] = entry.ChangedProperties.Select(c => c.PropertyName).ToList()
        };

        return System.Text.Json.JsonSerializer.Serialize(extra);
    }

    private static string? SerializeValue(object? value)
    {
        if (value == null) return null;
        if (value is DateTime dt) return dt.ToString("o");
        if (value is Enum e) return e.ToString();
        if (value is Guid g) return g.ToString();
        return value.ToString();
    }
}