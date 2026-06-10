using Autofac;
using Coast.Api.Engines.Bases;
using Coast.Api.Infrastructure.DataPersistence.EfCore;
using Coast.Api.Infrastructure.DataPersistence.EfCore.Entities.Bases;
using Microsoft.EntityFrameworkCore;

namespace Coast.Api.Engines.EfCoreEngines;
public class RegisterDbSet : IBuilderEngine
{
    private readonly ContainerBuilder _container;

    public RegisterDbSet(ContainerBuilder container)
    {
        _container = container;
    }

    public void Run()
    {
        // 注册 DbContext
        _container.RegisterType<ApplicationDbContext>()
            .AsSelf()
            .As<DbContext>()
            .InstancePerLifetimeScope();

        // 注册 DbContextFactory（用于创建独立作用域的 DbContext）
        _container.Register(context =>
        {
            var options = context.Resolve<DbContextOptions<ApplicationDbContext>>();
            return new ApplicationDbContextFactory(options);
        }).As<IDbContextFactory<ApplicationDbContext>>()
          .InstancePerLifetimeScope();

        var idbEntityType = typeof(IEfEntity<>);
        var idbEntityAssembly = idbEntityType.Assembly;
        var dbEntityTypes = idbEntityAssembly
            ?.ExportedTypes
            .Where(e => e.GetInterfaces().Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == idbEntityType) &&
                        e is { IsClass: true, IsAbstract: false })
            .ToArray();
        if (dbEntityTypes != null && dbEntityTypes.Any())
        {
            foreach (var dbEntityType in dbEntityTypes)
            {
                var dbAccessorType = typeof(DbAccessor<>).MakeGenericType(dbEntityType);
                _container
                    .RegisterType(dbAccessorType)
                    .AsSelf()
                    .InstancePerLifetimeScope();
            }
        }
    }
}

/// <summary>
/// ApplicationDbContext 工厂
/// </summary>
public class ApplicationDbContextFactory : IDbContextFactory<ApplicationDbContext>
{
    private readonly DbContextOptions<ApplicationDbContext> _options;

    public ApplicationDbContextFactory(DbContextOptions<ApplicationDbContext> options)
    {
        _options = options;
    }

    public ApplicationDbContext CreateDbContext()
    {
        return new ApplicationDbContext(_options);
    }

    public Task<ApplicationDbContext> CreateDbContextAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(CreateDbContext());
    }
}