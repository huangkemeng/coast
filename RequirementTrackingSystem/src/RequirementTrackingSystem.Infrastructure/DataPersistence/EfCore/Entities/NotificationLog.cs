using RequirementTrackingSystem.Infrastructure.DataPersistence.DataEntityBases;
using RequirementTrackingSystem.Infrastructure.DataPersistence.EfCore.Entities.Bases;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage;

namespace RequirementTrackingSystem.Infrastructure.DataPersistence.EfCore.Entities;

public class NotificationLog : IHasKey<int>, IEfEntity<NotificationLog>
{
    public int Id { get; set; }
    public int RequirementId { get; set; }
    public NotificationType Type { get; set; }
    public int RobotId { get; set; }
    public NotificationStatus Status { get; set; }
    public DateTime? SentAt { get; set; }
    public string? ErrorMessage { get; set; }
    public int RetryCount { get; set; }
    public DateTime? LastAttemptAt { get; set; }

    public Requirement? Requirement { get; set; }
    public Robot? Robot { get; set; }

    public static void ConfigureEntityMapping(EntityTypeBuilder<NotificationLog> builder, IRelationalTypeMappingSource mappingSource)
    {
        builder.AutoConfigure(mappingSource);
        builder.Property(e => e.ErrorMessage).HasMaxLength(1000);
        builder.HasOne(e => e.Requirement).WithMany().HasForeignKey(e => e.RequirementId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(e => e.Robot).WithMany().HasForeignKey(e => e.RobotId).OnDelete(DeleteBehavior.Restrict);
    }
}