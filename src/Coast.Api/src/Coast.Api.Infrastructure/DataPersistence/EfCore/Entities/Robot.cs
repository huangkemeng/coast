using Coast.Api.Infrastructure.DataPersistence.DataEntityBases;
using Coast.Api.Infrastructure.DataPersistence.EfCore.Entities.Bases;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage;

namespace Coast.Api.Infrastructure.DataPersistence.EfCore.Entities;

/// <summary>
/// 企业微信机器人实体
/// </summary>
public class Robot : IEfEntity<Robot>, IHasKey<int>, ICanSoftDelete, IHasCreatedOn, IHasCreator, IHasUpdater
{
    public Robot()
    {
        this.InitPropertyValues();
    }

    public static void ConfigureEntityMapping(EntityTypeBuilder<Robot> builder,
        IRelationalTypeMappingSource mappingSource)
    {
        builder.AutoConfigure(mappingSource);

        // 普通索引：机器人名称、启用状态
        builder.HasIndex(r => r.Name);
        builder.HasIndex(r => r.IsEnabled);

        // 配置字符串字段长度
        builder.Property(r => r.Name).HasMaxLength(50).IsRequired();
        builder.Property(r => r.WebhookUrl).HasMaxLength(500).IsRequired();
        builder.Property(r => r.GroupName).HasMaxLength(100);
    }

    /// <summary>主键，自增</summary>
    public int Id { get; set; }

    /// <summary>机器人名称</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Webhook 地址</summary>
    public string WebhookUrl { get; set; } = string.Empty;

    /// <summary>所属群组</summary>
    public string? GroupName { get; set; }

    /// <summary>启用状态</summary>
    public bool IsEnabled { get; set; }

    /// <summary>软删除标记</summary>
    public bool IsDeleted { get; set; }

    /// <summary>创建时间</summary>
    public DateTime CreatedOn { get; set; }

    /// <summary>创建人</summary>
    public Guid? CreatedBy { get; set; }

    /// <summary>更新时间</summary>
    public DateTime? UpdatedOn { get; set; }

    /// <summary>更新人</summary>
    public Guid? UpdatedBy { get; set; }

    // 导航属性
    /// <summary>需求集合导航属性</summary>
    public ICollection<Requirement> Requirements { get; set; } = new List<Requirement>();

    /// <summary>通知日志集合导航属性</summary>
    public ICollection<NotificationLog> NotificationLogs { get; set; } = new List<NotificationLog>();
}
