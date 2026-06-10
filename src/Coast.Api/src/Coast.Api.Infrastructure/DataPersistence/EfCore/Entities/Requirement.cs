using Coast.Api.Infrastructure.DataPersistence.DataEntityBases;
using Coast.Api.Infrastructure.DataPersistence.EfCore.Entities.Bases;
using Coast.Api.Infrastructure.DataPersistence.EfCore.Entities.Enums;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage;

namespace Coast.Api.Infrastructure.DataPersistence.EfCore.Entities;

/// <summary>
/// 需求核心实体
/// </summary>
public class Requirement : IEfEntity<Requirement>, IHasKey<int>, ICanSoftDelete, IHasCreatedOn, IHasCreator, IHasUpdater
{
    public Requirement()
    {
        this.InitPropertyValues();
    }

    public static void ConfigureEntityMapping(EntityTypeBuilder<Requirement> builder,
        IRelationalTypeMappingSource mappingSource)
    {
        builder.AutoConfigure(mappingSource);

        // 唯一索引：需求编号
        builder.HasIndex(r => r.Code).IsUnique();

        // 普通索引：状态、跟进人、项目、机器人、时间字段
        builder.HasIndex(r => r.Status);
        builder.HasIndex(r => r.FollowerId);
        builder.HasIndex(r => r.ProjectId);
        builder.HasIndex(r => r.RobotId);
        builder.HasIndex(r => r.PlanTestDate);
        builder.HasIndex(r => r.PlanOnlineDate);

        // 复合索引：状态+跟进人、状态+项目
        builder.HasIndex(r => new { r.Status, r.FollowerId });
        builder.HasIndex(r => new { r.Status, r.ProjectId });

        // 配置字符串字段长度
        builder.Property(r => r.Name).HasMaxLength(100).IsRequired();
        builder.Property(r => r.Code).HasMaxLength(50).IsRequired();
        builder.Property(r => r.DocumentUrl).HasMaxLength(500);
        builder.Property(r => r.Remark).HasMaxLength(500);

        // 配置小数字段精度
        builder.Property(r => r.Quote).HasPrecision(18, 2);
    }

    /// <summary>主键，自增</summary>
    public int Id { get; set; }

    /// <summary>需求名称</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>需求号，唯一标识</summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>当前状态</summary>
    public RequirementStatus Status { get; set; }

    /// <summary>进度 0-100</summary>
    public int? Progress { get; set; }

    /// <summary>跟进人</summary>
    public Guid FollowerId { get; set; }

    /// <summary>所属项目</summary>
    public int ProjectId { get; set; }

    /// <summary>通知机器人</summary>
    public int? RobotId { get; set; }

    /// <summary>优先级，默认 Medium</summary>
    public RequirementPriority Priority { get; set; }

    /// <summary>计划开始时间</summary>
    public DateTime? PlanStartDate { get; set; }

    /// <summary>计划交测时间</summary>
    public DateTime? PlanTestDate { get; set; }

    /// <summary>计划上线时间</summary>
    public DateTime? PlanOnlineDate { get; set; }

    /// <summary>实际交测时间</summary>
    public DateTime? ActualTestDate { get; set; }

    /// <summary>实际上线时间</summary>
    public DateTime? ActualOnlineDate { get; set; }

    /// <summary>需求已确认标记（系统自动控制）</summary>
    public bool IsConfirmed { get; set; }

    /// <summary>需求文档链接</summary>
    public string? DocumentUrl { get; set; }

    /// <summary>报价（仅管理员可见）</summary>
    public decimal? Quote { get; set; }

    /// <summary>备注</summary>
    public string? Remark { get; set; }

    /// <summary>并发控制版本号</summary>
    public int Version { get; set; }

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
    /// <summary>跟进人导航属性</summary>
    public ApplicationUser? Follower { get; set; }

    /// <summary>项目导航属性</summary>
    public Project? Project { get; set; }

    /// <summary>机器人导航属性</summary>
    public Robot? Robot { get; set; }

    /// <summary>通知日志集合导航属性</summary>
    public ICollection<NotificationLog> NotificationLogs { get; set; } = new List<NotificationLog>();
}
