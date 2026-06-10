using Coast.Api.Infrastructure.DataPersistence.DataEntityBases;
using Coast.Api.Infrastructure.DataPersistence.EfCore.Entities.Bases;
using Coast.Api.Infrastructure.DataPersistence.EfCore.Entities.Enums;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage;

namespace Coast.Api.Infrastructure.DataPersistence.EfCore.Entities;

/// <summary>
/// 操作日志实体 / 敏感操作审计（不实现软删除，历史记录需永久保留）
/// </summary>
public class OperationLog : IEfEntity<OperationLog>, IHasKey<int>, IHasCreatedOn
{
    public OperationLog()
    {
        this.InitPropertyValues();
    }

    public static void ConfigureEntityMapping(EntityTypeBuilder<OperationLog> builder,
        IRelationalTypeMappingSource mappingSource)
    {
        builder.AutoConfigure(mappingSource);

        // 普通索引：操作人、操作类型、操作对象、时间
        builder.HasIndex(o => o.OperatorId);
        builder.HasIndex(o => o.OperationType);
        builder.HasIndex(o => new { o.TargetType, o.TargetId });
        builder.HasIndex(o => o.CreatedOn);

        // 配置字符串字段长度
        builder.Property(o => o.OperatorName).HasMaxLength(100);
        builder.Property(o => o.TargetId).HasMaxLength(50);
        builder.Property(o => o.TargetName).HasMaxLength(200);
        builder.Property(o => o.IpAddress).HasMaxLength(50);
        builder.Property(o => o.UserAgent).HasMaxLength(500);
        builder.Property(o => o.Description).HasMaxLength(500);
        builder.Property(o => o.ExtraData).HasMaxLength(2000);
    }

    /// <summary>主键，自增</summary>
    public int Id { get; set; }

    /// <summary>操作人</summary>
    public Guid? OperatorId { get; set; }

    /// <summary>操作人姓名（冗余存储）</summary>
    public string? OperatorName { get; set; }

    /// <summary>操作类型</summary>
    public OperationType OperationType { get; set; }

    /// <summary>操作对象类型</summary>
    public TargetType TargetType { get; set; }

    /// <summary>操作对象ID</summary>
    public string? TargetId { get; set; }

    /// <summary>操作对象名称</summary>
    public string? TargetName { get; set; }

    /// <summary>IP 地址</summary>
    public string? IpAddress { get; set; }

    /// <summary>浏览器 User-Agent</summary>
    public string? UserAgent { get; set; }

    /// <summary>操作描述</summary>
    public string? Description { get; set; }

    /// <summary>额外数据（JSON格式）</summary>
    public string? ExtraData { get; set; }

    /// <summary>操作时间</summary>
    public DateTime CreatedOn { get; set; }

    /// <summary>会话ID（可选）</summary>
    public Guid? SessionId { get; set; }

    // 导航属性
    /// <summary>操作人导航属性</summary>
    public ApplicationUser? Operator { get; set; }

    /// <summary>会话导航属性</summary>
    public AuditSession? Session { get; set; }

    /// <summary>数据变更日志集合导航属性</summary>
    public ICollection<DataChangeLog> DataChangeLogs { get; set; } = new List<DataChangeLog>();
}
