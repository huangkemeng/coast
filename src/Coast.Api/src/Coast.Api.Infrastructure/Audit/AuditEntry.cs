using Coast.Api.Infrastructure.DataPersistence.EfCore.Entities.Enums;

namespace Coast.Api.Infrastructure.Audit;

/// <summary>
/// 实体变更条目
/// </summary>
public class AuditEntry
{
    /// <summary>操作类型</summary>
    public OperationType OperationType { get; set; }

    /// <summary>实体类型</summary>
    public TargetType EntityType { get; set; }

    /// <summary>实体ID</summary>
    public string EntityId { get; set; } = string.Empty;

    /// <summary>实体名称</summary>
    public string EntityName { get; set; } = string.Empty;

    /// <summary>变更前值（仅 Update/Delete）</summary>
    public Dictionary<string, object?> OldValues { get; set; } = new();

    /// <summary>变更后值（仅 Create/Update）</summary>
    public Dictionary<string, object?> NewValues { get; set; } = new();

    /// <summary>实际变更的字段</summary>
    public List<PropertyChange> ChangedProperties { get; set; } = new();

    /// <summary>审计上下文</summary>
    public AuditContext? AuditContext { get; set; }

    /// <summary>操作时间</summary>
    public DateTime OperationTime { get; set; } = DateTime.UtcNow;

    /// <summary>Entity Framework 跟踪实体</summary>
    public object? Entity { get; set; }
}

/// <summary>
/// 属性变更信息
/// </summary>
public class PropertyChange
{
    /// <summary>属性名</summary>
    public string PropertyName { get; set; } = string.Empty;

    /// <summary>属性显示名</summary>
    public string? PropertyDisplayName { get; set; }

    /// <summary>字段类型</summary>
    public ChangeFieldType FieldType { get; set; }

    /// <summary>旧值</summary>
    public object? OldValue { get; set; }

    /// <summary>新值</summary>
    public object? NewValue { get; set; }

    /// <summary>是否敏感字段（不记录详情）</summary>
    public bool IsSensitive { get; set; }
}