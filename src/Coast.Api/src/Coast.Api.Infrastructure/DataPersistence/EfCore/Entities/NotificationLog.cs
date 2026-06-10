using Coast.Api.Infrastructure.DataPersistence.DataEntityBases;
using Coast.Api.Infrastructure.DataPersistence.EfCore.Entities.Bases;
using Coast.Api.Infrastructure.DataPersistence.EfCore.Entities.Enums;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage;

namespace Coast.Api.Infrastructure.DataPersistence.EfCore.Entities;

/// <summary>
/// 通知日志实体（不实现软删除，历史记录需永久保留）
/// </summary>
public class NotificationLog : IEfEntity<NotificationLog>, IHasKey<int>, IHasCreatedOn, IHasCreator
{
    public NotificationLog()
    {
        this.InitPropertyValues();
    }

    public static void ConfigureEntityMapping(EntityTypeBuilder<NotificationLog> builder,
        IRelationalTypeMappingSource mappingSource)
    {
        builder.AutoConfigure(mappingSource);

        // 普通索引：需求、机器人、状态、时间字段
        builder.HasIndex(n => n.RequirementId);
        builder.HasIndex(n => n.RobotId);
        builder.HasIndex(n => n.Status);
        builder.HasIndex(n => n.CreatedOn);
        builder.HasIndex(n => n.SentAt);

        // 配置字符串字段长度
        builder.Property(n => n.Content).HasMaxLength(2000).IsRequired();
        builder.Property(n => n.ErrorMessage).HasMaxLength(500);
    }

    /// <summary>主键，自增</summary>
    public int Id { get; set; }

    /// <summary>关联需求</summary>
    public int RequirementId { get; set; }

    /// <summary>接收机器人</summary>
    public int? RobotId { get; set; }

    /// <summary>通知类型</summary>
    public NotificationType Type { get; set; }

    /// <summary>发送状态</summary>
    public NotificationStatus Status { get; set; }

    /// <summary>通知内容</summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>失败原因</summary>
    public string? ErrorMessage { get; set; }

    /// <summary>重试次数</summary>
    public int RetryCount { get; set; }

    /// <summary>发送时间</summary>
    public DateTime? SentAt { get; set; }

    /// <summary>创建时间</summary>
    public DateTime CreatedOn { get; set; }

    /// <summary>创建人</summary>
    public Guid? CreatedBy { get; set; }

    // 导航属性
    /// <summary>需求导航属性</summary>
    public Requirement? Requirement { get; set; }

    /// <summary>机器人导航属性</summary>
    public Robot? Robot { get; set; }
}
