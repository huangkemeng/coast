using Coast.Api.Infrastructure.DataPersistence.DataEntityBases;
using Coast.Api.Infrastructure.DataPersistence.EfCore.Entities.Bases;
using Coast.Api.Infrastructure.DataPersistence.EfCore.Entities.Enums;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage;

namespace Coast.Api.Infrastructure.DataPersistence.EfCore.Entities;

/// <summary>
/// 数据变更日志实体 / 字段变更明细（不实现软删除，历史记录需永久保留）
/// </summary>
public class DataChangeLog : IEfEntity<DataChangeLog>, IHasKey<int>, IHasCreatedOn
{
    public DataChangeLog()
    {
        this.InitPropertyValues();
    }

    public static void ConfigureEntityMapping(EntityTypeBuilder<DataChangeLog> builder,
        IRelationalTypeMappingSource mappingSource)
    {
        builder.AutoConfigure(mappingSource);

        // 普通索引：操作日志ID、实体类型+实体ID、字段名、时间
        builder.HasIndex(d => d.OperationLogId);
        builder.HasIndex(d => new { d.EntityType, d.EntityId });
        builder.HasIndex(d => d.FieldName);
        builder.HasIndex(d => d.CreatedOn);

        // 配置字符串字段长度
        builder.Property(d => d.EntityId).HasMaxLength(50).IsRequired();
        builder.Property(d => d.FieldName).HasMaxLength(100).IsRequired();
        builder.Property(d => d.FieldDisplayName).HasMaxLength(100);
        builder.Property(d => d.OldValue).HasMaxLength(500);
        builder.Property(d => d.NewValue).HasMaxLength(500);
    }

    /// <summary>主键，自增</summary>
    public int Id { get; set; }

    /// <summary>关联操作日志</summary>
    public int OperationLogId { get; set; }

    /// <summary>实体类型</summary>
    public TargetType EntityType { get; set; }

    /// <summary>实体ID</summary>
    public string EntityId { get; set; } = string.Empty;

    /// <summary>字段名</summary>
    public string FieldName { get; set; } = string.Empty;

    /// <summary>字段显示名</summary>
    public string? FieldDisplayName { get; set; }

    /// <summary>字段类型</summary>
    public ChangeFieldType FieldType { get; set; }

    /// <summary>修改前值</summary>
    public string? OldValue { get; set; }

    /// <summary>修改后值</summary>
    public string? NewValue { get; set; }

    /// <summary>变更时间</summary>
    public DateTime CreatedOn { get; set; }

    // 导航属性
    /// <summary>操作日志导航属性</summary>
    public OperationLog? OperationLog { get; set; }
}
