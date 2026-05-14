using System.Diagnostics;
using RequirementTrackingSystem.Infrastructure.Bases;
using RequirementTrackingSystem.Infrastructure.DataPersistence.EfCore.Bases;
using RequirementTrackingSystem.Infrastructure.DataPersistence.EfCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;

namespace RequirementTrackingSystem.Infrastructure.DataPersistence.EfCore;

public class ApplicationDbContext : DbContext
{
    private readonly DbSetting _dbSetting;
    private readonly SettingOptions _settingOptions;

    public DbSet<Requirement> Requirements { get; set; } = null!;
    public DbSet<Project> Projects { get; set; } = null!;
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Robot> Robots { get; set; } = null!;
    public DbSet<NotificationLog> NotificationLogs { get; set; } = null!;
    public DbSet<NotificationJob> NotificationJobs { get; set; } = null!;

    public ApplicationDbContext(DbSetting dbSetting, SettingOptions settingOptions)
    {
        _dbSetting = dbSetting;
        _settingOptions = settingOptions;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var connectString = _settingOptions.Scene == SceneOptions.Test
            ? _dbSetting.ConnectionStrings.IntegrationTest
            : _dbSetting.ConnectionStrings.WebApi;

        optionsBuilder.UseMySql(connectString, new MySqlServerVersion(new Version(8, 0)),
            options => { options.CommandTimeout(6000); });
        if (Debugger.IsAttached)
        {
            optionsBuilder.UseLoggerFactory(LoggerFactory.Create(builder => builder.AddDebug()));
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        var mappingSource = this.GetService<IRelationalTypeMappingSource>();
        modelBuilder.LoadFromEntityConfigure(mappingSource);
    }
}