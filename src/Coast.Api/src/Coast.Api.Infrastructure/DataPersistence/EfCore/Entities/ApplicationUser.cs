using Coast.Api.Infrastructure.DataPersistence.DataEntityBases;
using Coast.Api.Infrastructure.DataPersistence.EfCore.Entities.Bases;
using Coast.Api.Infrastructure.DataPersistence.EfCore.Entities.Enums;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage;

namespace Coast.Api.Infrastructure.DataPersistence.EfCore.Entities;

/// <summary>
/// 应用用户实体
/// </summary>
public class ApplicationUser : IEfEntity<ApplicationUser>, IHasKey<Guid>, ICanSoftDelete, IHasCreatedOn, IHasCreator, IHasUpdater
{
    public ApplicationUser()
    {
        this.InitPropertyValues();
    }

    public static void ConfigureEntityMapping(EntityTypeBuilder<ApplicationUser> builder,
        IRelationalTypeMappingSource mappingSource)
    {
        builder.AutoConfigure(mappingSource);

        // 唯一索引：用户名、手机号
        builder.HasIndex(u => u.Username).IsUnique();
        builder.HasIndex(u => u.PhoneNumber).IsUnique();

        // 配置字符串字段长度
        builder.Property(u => u.Username).HasMaxLength(50).IsRequired();
        builder.Property(u => u.PasswordHash).HasMaxLength(200).IsRequired();
        builder.Property(u => u.Name).HasMaxLength(100).IsRequired();
        builder.Property(u => u.PhoneNumber).HasMaxLength(20);
        builder.Property(u => u.Email).HasMaxLength(100);
    }

    /// <summary>主键</summary>
    public Guid Id { get; set; }

    /// <summary>用户名，登录账号</summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>密码哈希（bcrypt，强度12）</summary>
    public string PasswordHash { get; set; } = string.Empty;

    /// <summary>姓名</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>手机号</summary>
    public string? PhoneNumber { get; set; }

    /// <summary>邮箱（可选）</summary>
    public string? Email { get; set; }

    /// <summary>用户角色</summary>
    public UserRole Role { get; set; }

    /// <summary>用户状态</summary>
    public UserStatus Status { get; set; }

    /// <summary>是否首次登录</summary>
    public bool IsFirstLogin { get; set; }

    /// <summary>密码错误次数</summary>
    public int PasswordErrorCount { get; set; }

    /// <summary>锁定截止时间</summary>
    public DateTime? LockedUntil { get; set; }

    /// <summary>最后登录时间</summary>
    public DateTime? LastLoginOn { get; set; }

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
    /// <summary>登录日志集合导航属性</summary>
    public ICollection<LoginLog> LoginLogs { get; set; } = new List<LoginLog>();

    /// <summary>审计会话集合导航属性</summary>
    public ICollection<AuditSession> AuditSessions { get; set; } = new List<AuditSession>();

    /// <summary>操作日志集合导航属性</summary>
    public ICollection<OperationLog> OperationLogs { get; set; } = new List<OperationLog>();

    /// <summary>负责的需求集合导航属性</summary>
    public ICollection<Requirement> AssignedRequirements { get; set; } = new List<Requirement>();

    /// <summary>负责的项目集合导航属性</summary>
    public ICollection<Project> ManagedProjects { get; set; } = new List<Project>();
}