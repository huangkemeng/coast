using RequirementTrackingSystem.Infrastructure.DataPersistence.DataEntityBases;
using RequirementTrackingSystem.Infrastructure.DataPersistence.EfCore.Entities.Bases;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage;

namespace RequirementTrackingSystem.Infrastructure.DataPersistence.EfCore.Entities;

public class Requirement : IHasKey<int>, IEfEntity<Requirement>
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string RequirementNo { get; set; } = string.Empty;
    public RequirementStatus Status { get; set; }
    public int Progress { get; set; }
    public int FollowerId { get; set; }
    public DateTime? PlanStartDate { get; set; }
    public DateTime? PlanTestDate { get; set; }
    public DateTime? PlanLaunchDate { get; set; }
    public DateTime? ActualTestDate { get; set; }
    public DateTime? ActualLaunchDate { get; set; }
    public bool IsConfirmed { get; set; }
    public string? DocUrl { get; set; }
    public decimal? Price { get; set; }
    public int ProjectId { get; set; }
    public int? RobotId { get; set; }
    public Priority Priority { get; set; }
    public string? Remark { get; set; }
    public int Version { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public User? Follower { get; set; }
    public Project? Project { get; set; }
    public Robot? Robot { get; set; }

    public static void ConfigureEntityMapping(EntityTypeBuilder<Requirement> builder, IRelationalTypeMappingSource mappingSource)
    {
        builder.AutoConfigure(mappingSource);
        builder.Property(e => e.Name).HasMaxLength(100).IsRequired();
        builder.Property(e => e.RequirementNo).HasMaxLength(50).IsRequired();
        builder.Property(e => e.DocUrl).HasMaxLength(500);
        builder.Property(e => e.Price).HasPrecision(18, 2);
        builder.Property(e => e.Remark).HasMaxLength(1000);
        builder.HasIndex(e => e.RequirementNo).IsUnique();

        builder.HasOne(e => e.Follower).WithMany().HasForeignKey(e => e.FollowerId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(e => e.Project).WithMany().HasForeignKey(e => e.ProjectId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(e => e.Robot).WithMany().HasForeignKey(e => e.RobotId).OnDelete(DeleteBehavior.SetNull);
    }
}