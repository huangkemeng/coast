using Autofac;
using Coast.Api.Engines.Bases;
using Coast.Api.Infrastructure.Bases;
using Coast.Api.Infrastructure.DataPersistence.EfCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Coast.Api.DbMigration;

public class DbMigrationFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var builder = new ContainerBuilder();
        var container = builder.SceneBuildWithEngines(SceneOptions.WebApi);
        var sqlDbContext = container.Resolve<ApplicationDbContext>();
        return sqlDbContext;
    }
}