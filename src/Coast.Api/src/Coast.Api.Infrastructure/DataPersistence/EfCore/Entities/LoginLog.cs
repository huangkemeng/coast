using Coast.Api.Infrastructure.DataPersistence.DataEntityBases;
using Coast.Api.Infrastructure.DataPersistence.EfCore.Entities.Bases;
using Coast.Api.Infrastructure.DataPersistence.EfCore.Entities.Enums;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage;

namespace Coast.Api.Infrastructure.DataPersistence.EfCore.Entities;

/// <summary>
/// 登录日志实体（不实现软删除，历史记录需永久保留）
/// </summary>
public class LoginLog : IEfEntity<LoginLog>, IHasKey<int>, IHasCreatedOn
{
    public LoginLog()
    {
        this.InitPropertyValues();
    }

    public static void ConfigureEntityMapping(EntityTypeBuilder<LoginLog> builder,
        IRelationalTypeMappingSource mappingSource)
    {
        builder.AutoConfigure(mappingSource);

        // 普通索引：用户ID、用户名、登录时间
        builder.HasIndex(l => l.UserId);
        builder.HasIndex(l => l.Username);
        builder.HasIndex(l => l.CreatedOn);

        // 配置字符串字段长度
        builder.Property(l => l.Username).HasMaxLength(50).IsRequired();
        builder.Property(l => l.IpAddress).HasMaxLength(50);
        builder.Property(l => l.UserAgent).HasMaxLength(500);
        builder.Property(l => l.ErrorMessage).HasMaxLength(200);
    }

    /// <summary>主键，自增</summary>
    public int Id { get; set; }

    /// <summary>关联用户</summary>
    public Guid? UserId { get; set; }

    /// <summary>登录用户名</summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>登录状态</summary>
    public LoginStatus Status { get; set; }

    /// <summary>IP 地址</summary>
    public string? IpAddress { get; set; }

    /// <summary>浏览器 User-Agent</summary>
    public string? UserAgent { get; set; }

    /// <summary>错误信息</summary>
    public string? ErrorMessage { get; set; }

    /// <summary>登录时间</summary>
    public DateTime CreatedOn { get; set; }

    // 导航属性
    /// <summary>用户导航属性</summary>
    public ApplicationUser? User { get; set; }
}
