using Coast.Api.Infrastructure.DataPersistence.DataEntityBases;
using Coast.Api.Infrastructure.DataPersistence.EfCore.Entities.Bases;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage;

namespace Coast.Api.Infrastructure.DataPersistence.EfCore.Entities;

/// <summary>
/// 审计会话实体（用于会话管理和审计追踪）
/// </summary>
public class AuditSession : IEfEntity<AuditSession>, IHasKey<Guid>, IHasCreatedOn
{
    public AuditSession()
    {
        this.InitPropertyValues();
    }

    public static void ConfigureEntityMapping(EntityTypeBuilder<AuditSession> builder,
        IRelationalTypeMappingSource mappingSource)
    {
        builder.AutoConfigure(mappingSource);

        // 普通索引：用户ID、登录时间、最后活动时间、是否有效
        builder.HasIndex(s => s.UserId);
        builder.HasIndex(s => s.LoginAt);
        builder.HasIndex(s => s.LastActivityAt);
        builder.HasIndex(s => s.IsActive);

        // 配置字符串字段长度
        builder.Property(s => s.IpAddress).HasMaxLength(50);
        builder.Property(s => s.UserAgent).HasMaxLength(500);
        builder.Property(s => s.RefreshToken).HasMaxLength(200);
    }

    /// <summary>主键（会话ID）</summary>
    public Guid Id { get; set; }

    /// <summary>用户ID</summary>
    public Guid UserId { get; set; }

    /// <summary>登录IP</summary>
    public string? IpAddress { get; set; }

    /// <summary>浏览器 User-Agent</summary>
    public string? UserAgent { get; set; }

    /// <summary>Refresh Token（用于会话追踪）</summary>
    public string? RefreshToken { get; set; }

    /// <summary>登录时间</summary>
    public DateTime LoginAt { get; set; }

    /// <summary>最后活动时间</summary>
    public DateTime LastActivityAt { get; set; }

    /// <summary>会话过期时间</summary>
    public DateTime ExpiresAt { get; set; }

    /// <summary>会话是否有效</summary>
    public bool IsActive { get; set; }

    /// <summary>创建时间</summary>
    public DateTime CreatedOn { get; set; }

    // 导航属性
    /// <summary>用户导航属性</summary>
    public ApplicationUser? User { get; set; }

    /// <summary>操作日志集合导航属性</summary>
    public ICollection<OperationLog> OperationLogs { get; set; } = new List<OperationLog>();

    /// <summary>API访问日志集合导航属性</summary>
    public ICollection<ApiAccessLog> ApiAccessLogs { get; set; } = new List<ApiAccessLog>();
}
