using Coast.Api.Engines.Bases;
using Coast.Api.Infrastructure.Audit;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Coast.Api.Engines.AuditEngines;

/// <summary>
/// 配置审计中间件引擎（IAppEngine 在应用启动后执行）
/// </summary>
public class UseAuditMiddleware : IAppEngine
{
    public void Run()
    {
        // 无需执行，配置已通过扩展方法在 Program.cs 中调用
    }
}

/// <summary>
/// 审计中间件扩展方法
/// </summary>
public static class AuditMiddlewareExtensions
{
    /// <summary>
    /// 使用审计中间件
    /// </summary>
    public static WebApplication UseAuditMiddleware(this WebApplication app)
    {
        // 审计上下文中间件（必须最先执行，用于捕获用户信息）
        app.UseMiddleware<AuditContextMiddleware>();

        // API 访问日志中间件
        app.UseMiddleware<ApiAccessLogMiddleware>();

        return app;
    }
}