using Coast.Api.Infrastructure.Audit;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Coast.Api.Infrastructure;

/// <summary>
/// 审计服务扩展
/// </summary>
public static class AuditServiceExtensions
{
    /// <summary>
    /// 添加审计服务
    /// </summary>
    public static IServiceCollection AddAuditServices(this IServiceCollection services)
    {
        // 审计上下文访问器（使用 Scoped 生命周期）
        services.AddScoped<IAuditContextAccessor, AuditContextAccessor>();

        // 审计日志写入器
        services.AddScoped<IAuditLogWriter, AuditLogWriter>();

        // 实体过滤器
        services.AddScoped<IAuditEntityFilter, DefaultAuditEntityFilter>();
        services.AddScoped<IAuditEntityFilter, AttributeAuditEntityFilter>();

        // 敏感字段过滤器（单例）
        services.AddSingleton<SensitiveFieldFilter>();

        return services;
    }

    /// <summary>
    /// 添加审计拦截器到 DbContext
    /// </summary>
    public static DbContextOptionsBuilder AddAuditInterceptor(
        this DbContextOptionsBuilder optionsBuilder,
        IServiceProvider serviceProvider)
    {
        var interceptor = serviceProvider.GetRequiredService<AuditInterceptor>();
        optionsBuilder.AddInterceptors(interceptor);
        return optionsBuilder;
    }

    /// <summary>
    /// 配置审计中间件
    /// </summary>
    public static IApplicationBuilder UseAudit(this IApplicationBuilder app)
    {
        // 审计上下文中间件（必须最先执行，用于捕获用户信息）
        app.UseMiddleware<AuditContextMiddleware>();

        // API 访问日志中间件
        app.UseMiddleware<ApiAccessLogMiddleware>();

        return app;
    }
}