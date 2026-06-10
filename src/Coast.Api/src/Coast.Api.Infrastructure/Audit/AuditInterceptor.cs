using Coast.Api.Infrastructure.DataPersistence.EfCore.Entities;
using Coast.Api.Infrastructure.DataPersistence.EfCore.Entities.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Logging;

namespace Coast.Api.Infrastructure.Audit;

/// <summary>
/// EF Core SaveChanges 拦截器 - 自动记录审计日志（同步写入，保证线程安全）
/// </summary>
public class AuditInterceptor : SaveChangesInterceptor
{
    private readonly IAuditContextAccessor _auditContextAccessor;
    private readonly IEnumerable<IAuditEntityFilter> _entityFilters;
    private readonly SensitiveFieldFilter _sensitiveFieldFilter;
    private readonly ILogger<AuditInterceptor> _logger;

    public AuditInterceptor(
        IAuditContextAccessor auditContextAccessor,
        IEnumerable<IAuditEntityFilter> entityFilters,
        SensitiveFieldFilter sensitiveFieldFilter,
        ILogger<AuditInterceptor> logger)
    {
        _auditContextAccessor = auditContextAccessor;
        _entityFilters = entityFilters;
        _sensitiveFieldFilter = sensitiveFieldFilter;
        _logger = logger;
    }

    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        ProcessAudit(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        ProcessAudit(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void ProcessAudit(DbContext? context)
    {
        if (context == null) return;

        var auditContext = _auditContextAccessor.Current;
        var now = DateTime.UtcNow;

        // 获取所有被跟踪的实体
        var entries = context.ChangeTracker.Entries().ToList();

        foreach (var entry in entries)
        {
            var entityType = entry.Entity.GetType();

            // 检查是否应该跳过
            if (ShouldSkip(entityType)) continue;

            var auditEntry = CreateAuditEntry(entry, now);
            if (auditEntry != null)
            {
                auditEntry.AuditContext = auditContext;

                // 同步写入（在同一个 DbContext 中，确保线程安全）
                WriteAuditEntry(context, auditEntry);
            }
        }
    }

    private bool ShouldSkip(Type entityType)
    {
        return _entityFilters.Any(f => f.ShouldSkip(entityType));
    }

    private AuditEntry? CreateAuditEntry(Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry entry, DateTime now)
    {
        var entityType = entry.Entity.GetType();
        var entityId = GetEntityId(entry);
        var entityName = entityType.Name;

        var auditEntry = new AuditEntry
        {
            Entity = entry.Entity,
            EntityId = entityId,
            EntityName = entityName,
            EntityType = GetTargetType(entityType),
            OperationTime = now
        };

        switch (entry.State)
        {
            case EntityState.Added:
                auditEntry.OperationType = OperationType.Create;
                CaptureNewValues(entry, auditEntry);
                break;

            case EntityState.Modified:
                auditEntry.OperationType = OperationType.Update;
                CaptureOldAndNewValues(entry, auditEntry);
                break;

            case EntityState.Deleted:
                auditEntry.OperationType = OperationType.Delete;
                CaptureOldValues(entry, auditEntry);
                break;

            default:
                return null;
        }

        // 如果没有实际变更，跳过
        if (auditEntry.OperationType == OperationType.Update && !auditEntry.ChangedProperties.Any())
        {
            return null;
        }

        return auditEntry;
    }

    private static string GetEntityId(Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry entry)
    {
        var key = entry.Metadata.FindPrimaryKey();
        if (key == null) return string.Empty;

        var keyValue = entry.Property(key.Properties.First().Name).CurrentValue;
        return keyValue?.ToString() ?? string.Empty;
    }

    private static TargetType GetTargetType(Type entityType)
    {
        return entityType.Name switch
        {
            nameof(Requirement) => TargetType.Requirement,
            nameof(Project) => TargetType.Project,
            nameof(Robot) => TargetType.Robot,
            nameof(ApplicationUser) => TargetType.User,
            _ => TargetType.System
        };
    }

    private void CaptureNewValues(Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry entry, AuditEntry auditEntry)
    {
        foreach (var property in entry.Properties)
        {
            if (IsNavigationProperty(property.Metadata)) continue;

            var propertyName = property.Metadata.Name;
            var value = property.CurrentValue;

            auditEntry.NewValues[propertyName] = value;

            // 跳过系统字段
            if (propertyName is "CreatedOn" or "UpdatedOn" or "Id")
            {
                continue;
            }

            auditEntry.ChangedProperties.Add(new PropertyChange
            {
                PropertyName = propertyName,
                FieldType = GetFieldType(property.Metadata.ClrType),
                NewValue = value,
                OldValue = null,
                IsSensitive = _sensitiveFieldFilter.IsSensitiveField(propertyName)
            });
        }
    }

    private void CaptureOldValues(Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry entry, AuditEntry auditEntry)
    {
        foreach (var property in entry.Properties)
        {
            if (IsNavigationProperty(property.Metadata)) continue;

            var propertyName = property.Metadata.Name;
            var value = property.OriginalValue;

            auditEntry.OldValues[propertyName] = value;
        }
    }

    private void CaptureOldAndNewValues(Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry entry, AuditEntry auditEntry)
    {
        foreach (var property in entry.Properties)
        {
            if (IsNavigationProperty(property.Metadata)) continue;

            var propertyName = property.Metadata.Name;
            var originalValue = property.OriginalValue;
            var currentValue = property.CurrentValue;

            auditEntry.OldValues[propertyName] = originalValue;
            auditEntry.NewValues[propertyName] = currentValue;

            // 跳过未变化的属性
            if (Equals(originalValue, currentValue)) continue;

            // 跳过系统字段
            if (propertyName is "UpdatedOn" or "Version" or "Id")
            {
                continue;
            }

            auditEntry.ChangedProperties.Add(new PropertyChange
            {
                PropertyName = propertyName,
                FieldType = GetFieldType(property.Metadata.ClrType),
                OldValue = originalValue,
                NewValue = currentValue,
                IsSensitive = _sensitiveFieldFilter.IsSensitiveField(propertyName)
            });
        }
    }

    private static bool IsNavigationProperty(IProperty property)
    {
        return property.IsForeignKey() ||
               property.IsShadowProperty() ||
               property.IsKey();
    }

    private static ChangeFieldType GetFieldType(Type clrType)
    {
        if (clrType == typeof(string)) return ChangeFieldType.Text;
        if (clrType == typeof(int) || clrType == typeof(long) || clrType == typeof(short) ||
            clrType == typeof(decimal) || clrType == typeof(double) || clrType == typeof(float))
            return ChangeFieldType.Number;
        if (clrType == typeof(DateTime) || clrType == typeof(DateTime?))
            return ChangeFieldType.DateTime;
        if (clrType.IsEnum) return ChangeFieldType.Enum;
        if (clrType == typeof(bool) || clrType == typeof(bool?)) return ChangeFieldType.Boolean;
        if (clrType == typeof(Guid) || clrType == typeof(Guid?)) return ChangeFieldType.ForeignKey;
        return ChangeFieldType.Text;
    }

    /// <summary>
    /// 同步写入审计日志（在同一个 DbContext 中，确保线程安全）
    /// </summary>
    private void WriteAuditEntry(DbContext context, AuditEntry entry)
    {
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

            context.Set<OperationLog>().Add(operationLog);

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

                    context.Set<DataChangeLog>().Add(dataChangeLog);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "写入审计日志失败: {EntityType} {EntityId}", entry.EntityType, entry.EntityId);
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