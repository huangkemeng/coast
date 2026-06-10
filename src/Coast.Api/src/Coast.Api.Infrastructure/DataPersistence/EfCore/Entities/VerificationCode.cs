using Coast.Api.Infrastructure.DataPersistence.DataEntityBases;
using Coast.Api.Infrastructure.DataPersistence.EfCore.Entities.Bases;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage;

namespace Coast.Api.Infrastructure.DataPersistence.EfCore.Entities;

/// <summary>
/// 短信验证码实体
/// </summary>
public class VerificationCode : IEfEntity<VerificationCode>, IHasKey<int>, ICanSoftDelete
{
    public VerificationCode()
    {
        this.InitPropertyValues();
    }

    public static void ConfigureEntityMapping(EntityTypeBuilder<VerificationCode> builder,
        IRelationalTypeMappingSource mappingSource)
    {
        builder.AutoConfigure(mappingSource);

        // 普通索引：手机号、验证码+类型、过期时间
        builder.HasIndex(v => v.PhoneNumber);
        builder.HasIndex(v => new { v.Code, v.Type });
        builder.HasIndex(v => v.ExpiresAt);

        // 配置字符串字段长度
        builder.Property(v => v.PhoneNumber).HasMaxLength(20).IsRequired();
        builder.Property(v => v.Code).HasMaxLength(10).IsRequired();
        builder.Property(v => v.Type).HasMaxLength(20).IsRequired();
    }

    /// <summary>主键，自增</summary>
    public int Id { get; set; }

    /// <summary>手机号</summary>
    public string PhoneNumber { get; set; } = string.Empty;

    /// <summary>验证码</summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>验证码类型</summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>过期时间</summary>
    public DateTime ExpiresAt { get; set; }

    /// <summary>使用时间</summary>
    public DateTime? UsedAt { get; set; }

    /// <summary>使用次数</summary>
    public int UsedCount { get; set; }

    /// <summary>今日发送次数</summary>
    public int SendCount { get; set; }

    /// <summary>最后发送时间</summary>
    public DateTime LastSentOn { get; set; }

    /// <summary>软删除标记</summary>
    public bool IsDeleted { get; set; }

    /// <summary>创建时间</summary>
    public DateTime CreatedOn { get; set; }
}
