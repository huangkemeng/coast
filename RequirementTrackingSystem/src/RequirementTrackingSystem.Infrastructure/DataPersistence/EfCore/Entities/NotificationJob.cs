using RequirementTrackingSystem.Infrastructure.DataPersistence.DataEntityBases;
using RequirementTrackingSystem.Infrastructure.DataPersistence.EfCore.Entities.Bases;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage;

namespace RequirementTrackingSystem.Infrastructure.DataPersistence.EfCore.Entities;

public class NotificationJob : IHasKey<int>, IEfEntity<NotificationJob>
{
    public int Id { get; set; }
    public int RequirementId { get; set; }
    public NotificationType Type { get; set; }
    public DateTime ScheduledAt { get; set; }
    public DateTime? SentAt { get; set; }
    public NotificationStatus Status { get; set; }

    public Requirement? Requirement { get; set; }

    public static void ConfigureEntityMapping(EntityTypeBuilder<NotificationJob> builder, IRelationalTypeMappingSource mappingSource)
    {
        builder.AutoConfigure(mappingSource);
        builder.HasOne(e => e.Requirement).WithMany().HasForeignKey(e => e.RequirementId).OnDelete(DeleteBehavior.Cascade);
    }
}