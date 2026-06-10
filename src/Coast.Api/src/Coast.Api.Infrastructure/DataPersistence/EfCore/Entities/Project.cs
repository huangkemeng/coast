using Coast.Api.Infrastructure.DataPersistence.DataEntityBases;
using Coast.Api.Infrastructure.DataPersistence.EfCore.Entities.Bases;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage;

namespace Coast.Api.Infrastructure.DataPersistence.EfCore.Entities;

/// <summary>
/// 项目实体
/// </summary>
public class Project : IEfEntity<Project>, IHasKey<int>, ICanSoftDelete, IHasCreatedOn, IHasCreator, IHasUpdater
{
    public Project()
    {
        this.InitPropertyValues();
    }

    public static void ConfigureEntityMapping(EntityTypeBuilder<Project> builder,
        IRelationalTypeMappingSource mappingSource)
    {
        builder.AutoConfigure(mappingSource);

        // 唯一索引：项目名称
        builder.HasIndex(p => p.Name).IsUnique();

        // 普通索引：项目编码、负责人
        builder.HasIndex(p => p.Code);
        builder.HasIndex(p => p.ManagerId);

        // 配置字符串字段长度
        builder.Property(p => p.Name).HasMaxLength(100).IsRequired();
        builder.Property(p => p.Code).HasMaxLength(50);
        builder.Property(p => p.Description).HasMaxLength(500);
    }

    /// <summary>主键，自增</summary>
    public int Id { get; set; }

    /// <summary>项目名称</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>项目编码</summary>
    public string? Code { get; set; }

    /// <summary>项目负责人</summary>
    public Guid? ManagerId { get; set; }

    /// <summary>项目描述</summary>
    public string? Description { get; set; }

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
    /// <summary>项目负责人导航属性</summary>
    public ApplicationUser? Manager { get; set; }

    /// <summary>需求集合导航属性</summary>
    public ICollection<Requirement> Requirements { get; set; } = new List<Requirement>();
}
