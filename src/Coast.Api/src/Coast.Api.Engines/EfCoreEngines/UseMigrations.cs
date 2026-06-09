using Autofac;
using Coast.Api.Engines.Bases;
using Coast.Api.Infrastructure.DataPersistence.EfCore;
using Microsoft.EntityFrameworkCore;

namespace Coast.Api.Engines.EfCoreEngines;

public class UseMigrations : IAppEngine
{
    private readonly ILifetimeScope migrateScope;

    public UseMigrations(ILifetimeScope migrateScope)
    {
        this.migrateScope = migrateScope;
    }

    public void Run()
    {
        var dbContext = migrateScope.Resolve<ApplicationDbContext>();
        var connectString = dbContext.Database.GetConnectionString();
        if (!string.IsNullOrWhiteSpace(connectString))
        {
            var migrations = dbContext.Database.GetMigrations();
            if (migrations.Any() && dbContext.Database.CanConnect())
            {
                dbContext.Database.Migrate();
            }
        }
    }
}