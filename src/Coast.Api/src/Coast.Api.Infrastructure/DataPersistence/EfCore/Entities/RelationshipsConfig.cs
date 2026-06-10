using Coast.Api.Infrastructure.DataPersistence.EfCore.Entities.Bases;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage;

namespace Coast.Api.Infrastructure.DataPersistence.EfCore.Entities;

/// <summary>
/// 实体关系配置
/// </summary>
public static class EntityRelationshipsConfig
{
    /// <summary>
    /// 配置所有实体之间的关系
    /// </summary>
    public static void ConfigureRelationships(ModelBuilder modelBuilder)
    {
        ConfigureProjectRelationships(modelBuilder);
        ConfigureRequirementRelationships(modelBuilder);
        ConfigureRobotRelationships(modelBuilder);
        ConfigureNotificationLogRelationships(modelBuilder);
        ConfigureLoginLogRelationships(modelBuilder);
        ConfigureAuditSessionRelationships(modelBuilder);
        ConfigureOperationLogRelationships(modelBuilder);
        ConfigureDataChangeLogRelationships(modelBuilder);
        ConfigureApiAccessLogRelationships(modelBuilder);
    }

    private static void ConfigureProjectRelationships(ModelBuilder modelBuilder)
    {
        // Project.ManagerId -> ApplicationUser.Id (可选)
        modelBuilder.Entity<Project>()
            .HasOne(p => p.Manager)
            .WithMany(u => u.ManagedProjects)
            .HasForeignKey(p => p.ManagerId)
            .OnDelete(DeleteBehavior.SetNull);
    }

    private static void ConfigureRequirementRelationships(ModelBuilder modelBuilder)
    {
        // Requirement.FollowerId -> ApplicationUser.Id (必需)
        modelBuilder.Entity<Requirement>()
            .HasOne(r => r.Follower)
            .WithMany(u => u.AssignedRequirements)
            .HasForeignKey(r => r.FollowerId)
            .OnDelete(DeleteBehavior.Restrict);

        // Requirement.ProjectId -> Project.Id (必需)
        modelBuilder.Entity<Requirement>()
            .HasOne(r => r.Project)
            .WithMany(p => p.Requirements)
            .HasForeignKey(r => r.ProjectId)
            .OnDelete(DeleteBehavior.Restrict);

        // Requirement.RobotId -> Robot.Id (可选，删除时置空)
        modelBuilder.Entity<Requirement>()
            .HasOne(r => r.Robot)
            .WithMany(rt => rt.Requirements)
            .HasForeignKey(r => r.RobotId)
            .OnDelete(DeleteBehavior.SetNull);
    }

    private static void ConfigureRobotRelationships(ModelBuilder modelBuilder)
    {
        // Robot 已配置与 Requirement 的关系
    }

    private static void ConfigureNotificationLogRelationships(ModelBuilder modelBuilder)
    {
        // NotificationLog.RequirementId -> Requirement.Id (必需)
        modelBuilder.Entity<NotificationLog>()
            .HasOne(n => n.Requirement)
            .WithMany(r => r.NotificationLogs)
            .HasForeignKey(n => n.RequirementId)
            .OnDelete(DeleteBehavior.Restrict);

        // NotificationLog.RobotId -> Robot.Id (可选)
        modelBuilder.Entity<NotificationLog>()
            .HasOne(n => n.Robot)
            .WithMany(rt => rt.NotificationLogs)
            .HasForeignKey(n => n.RobotId)
            .OnDelete(DeleteBehavior.SetNull);
    }

    private static void ConfigureLoginLogRelationships(ModelBuilder modelBuilder)
    {
        // LoginLog.UserId -> ApplicationUser.Id (可选)
        modelBuilder.Entity<LoginLog>()
            .HasOne(l => l.User)
            .WithMany(u => u.LoginLogs)
            .HasForeignKey(l => l.UserId)
            .OnDelete(DeleteBehavior.SetNull);
    }

    private static void ConfigureAuditSessionRelationships(ModelBuilder modelBuilder)
    {
        // AuditSession.UserId -> ApplicationUser.Id (必需)
        modelBuilder.Entity<AuditSession>()
            .HasOne(s => s.User)
            .WithMany(u => u.AuditSessions)
            .HasForeignKey(s => s.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }

    private static void ConfigureOperationLogRelationships(ModelBuilder modelBuilder)
    {
        // OperationLog.OperatorId -> ApplicationUser.Id (可选)
        modelBuilder.Entity<OperationLog>()
            .HasOne(o => o.Operator)
            .WithMany(u => u.OperationLogs)
            .HasForeignKey(o => o.OperatorId)
            .OnDelete(DeleteBehavior.SetNull);

        // OperationLog.SessionId -> AuditSession.Id (可选)
        modelBuilder.Entity<OperationLog>()
            .HasOne(o => o.Session)
            .WithMany(s => s.OperationLogs)
            .HasForeignKey(o => o.SessionId)
            .OnDelete(DeleteBehavior.SetNull);
    }

    private static void ConfigureDataChangeLogRelationships(ModelBuilder modelBuilder)
    {
        // DataChangeLog.OperationLogId -> OperationLog.Id (必需，级联删除)
        modelBuilder.Entity<DataChangeLog>()
            .HasOne(d => d.OperationLog)
            .WithMany(o => o.DataChangeLogs)
            .HasForeignKey(d => d.OperationLogId)
            .OnDelete(DeleteBehavior.Cascade);
    }

    private static void ConfigureApiAccessLogRelationships(ModelBuilder modelBuilder)
    {
        // ApiAccessLog.SessionId -> AuditSession.Id (可选)
        modelBuilder.Entity<ApiAccessLog>()
            .HasOne(a => a.Session)
            .WithMany(s => s.ApiAccessLogs)
            .HasForeignKey(a => a.SessionId)
            .OnDelete(DeleteBehavior.SetNull);

        // ApiAccessLog.UserId -> ApplicationUser.Id (可选)
        modelBuilder.Entity<ApiAccessLog>()
            .HasOne(a => a.User)
            .WithMany()
            .HasForeignKey(a => a.UserId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}