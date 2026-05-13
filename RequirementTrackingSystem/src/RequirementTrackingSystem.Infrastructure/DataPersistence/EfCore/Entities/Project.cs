using RequirementTrackingSystem.Infrastructure.DataPersistence.DataEntityBases;
using RequirementTrackingSystem.Infrastructure.DataPersistence.EfCore.Entities.Bases;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage;

namespace RequirementTrackingSystem.Infrastructure.DataPersistence.EfCore.Entities;

public class Project : IHasKey<int>, IEfEntity<Project>
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public int ManagerId { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public User? Manager { get; set; }

    public static void ConfigureEntityMapping(EntityTypeBuilder<Project> builder, IRelationalTypeMappingSource mappingSource)
    {
        builder.AutoConfigure(mappingSource);
        builder.Property(e => e.Name).HasMaxLength(100).IsRequired();
        builder.Property(e => e.Code).HasMaxLength(50).IsRequired();
        builder.Property(e => e.Description).HasMaxLength(500);
        builder.HasIndex(e => e.Code).IsUnique();
        builder.HasOne(e => e.Manager).WithMany().HasForeignKey(e => e.ManagerId).OnDelete(DeleteBehavior.Restrict);
    }
}