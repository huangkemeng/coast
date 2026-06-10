using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace Coast.Api.Infrastructure.Audit;

/// <summary>
/// 审计上下文中间件 - 为每个请求注入审计上下文
/// </summary>
public class AuditContextMiddleware
{
    private readonly RequestDelegate _next;

    public AuditContextMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IAuditContextAccessor auditContextAccessor)
    {
        try
        {
            // 设置审计上下文
            var auditContext = new AuditContext
            {
                UserId = GetUserId(context),
                UserName = GetUserName(context),
                SessionId = GetSessionId(context),
                IpAddress = GetIpAddress(context),
                UserAgent = GetUserAgent(context),
                RequestPath = context.Request.Path.Value,
                OperationTime = DateTime.UtcNow
            };

            auditContextAccessor.Current = auditContext;

            await _next(context);
        }
        finally
        {
            // 请求结束后清除上下文
            auditContextAccessor.Current = null;
        }
    }

    private static Guid? GetUserId(HttpContext context)
    {
        var claim = context.User?.FindFirst(ClaimTypes.NameIdentifier)
            ?? context.User?.FindFirst("sub");

        if (claim != null && Guid.TryParse(claim.Value, out var id))
        {
            return id;
        }
        return null;
    }

    private static string? GetUserName(HttpContext context)
    {
        return context.User?.FindFirst(ClaimTypes.Name)?.Value
            ?? context.User?.FindFirst("name")?.Value
            ?? context.User?.FindFirst(ClaimTypes.Email)?.Value;
    }

    private static Guid? GetSessionId(HttpContext context)
    {
        // 从 Items 中获取会话ID（由登录服务设置）
        if (context.Items.TryGetValue("SessionId", out var sessionId) && sessionId is Guid guid)
        {
            return guid;
        }

        // 从 Claims 中获取
        var claim = context.User?.FindFirst("session_id");
        if (claim != null && Guid.TryParse(claim.Value, out var id))
        {
            return id;
        }

        return null;
    }

    private static string? GetIpAddress(HttpContext context)
    {
        // 优先从 X-Forwarded-For 获取（反向代理场景）
        var forwardedFor = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (!string.IsNullOrEmpty(forwardedFor))
        {
            return forwardedFor.Split(',').First().Trim();
        }

        return context.Connection.RemoteIpAddress?.ToString();
    }

    private static string? GetUserAgent(HttpContext context)
    {
        return context.Request.Headers.UserAgent.FirstOrDefault();
    }
}