using Autofac;
using Coast.Api.Engines.Bases;
using Coast.Api.Infrastructure.Audit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Coast.Api.Engines.AuditEngines;

/// <summary>
/// 注册审计服务引擎（IBuilderEngine 在构建时执行）
/// </summary>
public class RegisterAuditServices : IBuilderEngine
{
    private readonly ContainerBuilder _container;

    public RegisterAuditServices(ContainerBuilder container)
    {
        _container = container;
    }

    public void Run()
    {
        // 注册审计上下文访问器
        _container.RegisterType<AuditContextAccessor>()
            .As<IAuditContextAccessor>()
            .InstancePerLifetimeScope();

        // 注册审计日志写入器
        _container.RegisterType<AuditLogWriter>()
            .As<IAuditLogWriter>()
            .InstancePerLifetimeScope();

        // 注册实体过滤器
        _container.RegisterType<DefaultAuditEntityFilter>()
            .As<IAuditEntityFilter>()
            .InstancePerLifetimeScope();

        _container.RegisterType<AttributeAuditEntityFilter>()
            .As<IAuditEntityFilter>()
            .InstancePerLifetimeScope();

        // 注册敏感字段过滤器（单例）
        _container.RegisterType<SensitiveFieldFilter>()
            .SingleInstance();

        // 注册审计拦截器（单例）
        _container.RegisterType<AuditInterceptor>()
            .SingleInstance();
    }
}