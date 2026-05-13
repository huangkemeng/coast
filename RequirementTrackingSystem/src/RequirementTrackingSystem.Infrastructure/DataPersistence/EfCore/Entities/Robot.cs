using RequirementTrackingSystem.Infrastructure.DataPersistence.DataEntityBases;
using RequirementTrackingSystem.Infrastructure.DataPersistence.EfCore.Entities.Bases;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage;

namespace RequirementTrackingSystem.Infrastructure.DataPersistence.EfCore.Entities;

public class Robot : IHasKey<int>, IEfEntity<Robot>
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string WebhookUrl { get; set; } = string.Empty;
    public string? GroupName { get; set; }
    public bool IsEnabled { get; set; } = true;
    public DateTime CreatedAt { get; set; }

    public static void ConfigureEntityMapping(EntityTypeBuilder<Robot> builder, IRelationalTypeMappingSource mappingSource)
    {
        builder.AutoConfigure(mappingSource);
        builder.Property(e => e.Name).HasMaxLength(100).IsRequired();
        builder.Property(e => e.WebhookUrl).HasMaxLength(500).IsRequired();
        builder.Property(e => e.GroupName).HasMaxLength(100);
    }
}