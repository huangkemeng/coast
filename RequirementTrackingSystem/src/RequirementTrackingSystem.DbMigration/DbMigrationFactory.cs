using Autofac;
using RequirementTrackingSystem.Engines.Bases;
using RequirementTrackingSystem.Infrastructure.Bases;
using RequirementTrackingSystem.Infrastructure.DataPersistence.EfCore;
using Microsoft.EntityFrameworkCore.Design;

namespace RequirementTrackingSystem.DbMigration;

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