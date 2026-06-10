using System.Reflection;

namespace Coast.Api.Infrastructure.Audit;

/// <summary>
/// 审计实体过滤器 - 控制哪些实体需要审计
/// </summary>
public interface IAuditEntityFilter
{
    /// <summary>是否跳过指定类型的审计</summary>
    bool ShouldSkip(Type entityType);
}

/// <summary>
/// 默认过滤器 - 跳过审计日志实体本身
/// </summary>
public class DefaultAuditEntityFilter : IAuditEntityFilter
{
    private static readonly HashSet<Type> SkipTypes = new()
    {
        typeof(DataPersistence.EfCore.Entities.OperationLog),
        typeof(DataPersistence.EfCore.Entities.DataChangeLog),
        typeof(DataPersistence.EfCore.Entities.ApiAccessLog),
        typeof(DataPersistence.EfCore.Entities.LoginLog),
        typeof(DataPersistence.EfCore.Entities.AuditSession),
        typeof(DataPersistence.EfCore.Entities.NotificationLog),
        typeof(DataPersistence.EfCore.Entities.VerificationCode)
    };

    public virtual bool ShouldSkip(Type entityType)
    {
        return SkipTypes.Any(t => t.IsAssignableFrom(entityType));
    }
}

/// <summary>
/// 自定义审计属性 - 标记实体是否需要审计
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class AuditAttribute : Attribute
{
    /// <summary>是否启用审计</summary>
    public bool Enabled { get; set; } = true;
}

/// <summary>
/// 跳过审计属性 - 标记实体跳过审计
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class SkipAuditAttribute : Attribute
{
}

/// <summary>
/// 基于属性的过滤器
/// </summary>
public class AttributeAuditEntityFilter : IAuditEntityFilter
{
    public bool ShouldSkip(Type entityType)
    {
        // 检查 SkipAudit 属性
        if (entityType.GetCustomAttribute<SkipAuditAttribute>() != null)
            return true;

        // 检查 Audit 属性
        var auditAttr = entityType.GetCustomAttribute<AuditAttribute>();
        return auditAttr?.Enabled == false;
    }
}

/// <summary>
/// 敏感字段属性 - 标记字段为敏感信息，审计时不记录具体值
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class SensitiveFieldAttribute : Attribute
{
}

/// <summary>
/// 敏感字段过滤器
/// </summary>
public class SensitiveFieldFilter
{
    private static readonly HashSet<string> SensitiveFields = new(StringComparer.OrdinalIgnoreCase)
    {
        "PasswordHash", "Password", "SecretKey", "ApiKey", "Token", "Secret"
    };

    public bool IsSensitiveField(string propertyName)
    {
        return SensitiveFields.Contains(propertyName) ||
               propertyName.EndsWith("Password", StringComparison.OrdinalIgnoreCase) ||
               propertyName.EndsWith("Secret", StringComparison.OrdinalIgnoreCase) ||
               propertyName.EndsWith("Key", StringComparison.OrdinalIgnoreCase);
    }
}