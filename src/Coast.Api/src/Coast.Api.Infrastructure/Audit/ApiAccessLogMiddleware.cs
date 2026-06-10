using System.Diagnostics;
using System.Text.RegularExpressions;
using Coast.Api.Infrastructure.DataPersistence.EfCore;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Coast.Api.Infrastructure.Audit;

/// <summary>
/// API 访问日志中间件 - 自动记录所有 API 访问
/// </summary>
public class ApiAccessLogMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ApiAccessLogMiddleware> _logger;

    // 跳过记录静态文件和健康检查
    private static readonly HashSet<string> SkipPrefixes = new(StringComparer.OrdinalIgnoreCase)
    {
        "/health", "/favicon.ico", "/swagger", "/swagger-ui", "/api-docs",
        "/metrics", "/_health", "/hangfire"
    };

    // 敏感字段，用于脱敏
    private static readonly string[] SensitiveFields = { "password", "token", "secret", "key", "authorization", "credential" };

    public ApiAccessLogMiddleware(RequestDelegate next, ILogger<ApiAccessLogMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // 跳过不需要记录的请求
        if (ShouldSkip(context.Request.Path))
        {
            await _next(context);
            return;
        }

        var stopwatch = Stopwatch.StartNew();
        var ipAddress = GetIpAddress(context);
        var userAgent = context.Request.Headers.UserAgent.ToString();

        // 获取用户和会话信息
        Guid? userId = null;
        Guid? sessionId = null;

        if (context.User?.Identity?.IsAuthenticated == true)
        {
            var userIdClaim = context.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)
                ?? context.User.FindFirst("sub");
            if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out var id))
            {
                userId = id;
            }

            if (context.Items.TryGetValue("SessionId", out var sid) && sid is Guid sidGuid)
            {
                sessionId = sidGuid;
            }
        }

        // 捕获请求体（脱敏）
        string? requestBody = null;
        if (HasRequestBody(context.Request))
        {
            requestBody = await CaptureRequestBodyAsync(context.Request);
        }

        Exception? exception = null;
        int statusCode = 0;

        // 执行请求
        try
        {
            await _next(context);
            statusCode = context.Response.StatusCode;
        }
        catch (Exception ex)
        {
            exception = ex;
            statusCode = 500;
            throw;
        }
        finally
        {
            stopwatch.Stop();

            // 提取字段值为局部变量，避免闭包问题
            var logSessionId = sessionId;
            var logUserId = userId;
            var logHttpMethod = context.Request.Method;
            var logApiPath = context.Request.Path.Value ?? string.Empty;
            var logQueryString = context.Request.QueryString.HasValue ? context.Request.QueryString.Value : null;
            var logRequestBody = requestBody;
            var logStatusCode = statusCode;
            var logResponseTimeMs = (int)stopwatch.ElapsedMilliseconds;
            var logIpAddress = ipAddress;
            var logUserAgent = userAgent;
            var logErrorMessage = exception?.Message;
            var logCreatedOn = DateTime.UtcNow;

            // 获取 IServiceScopeFactory 用于创建独立作用域
            var scopeFactory = context.RequestServices.GetRequiredService<IServiceScopeFactory>();

            // 异步写入（不阻塞请求）
            _ = Task.Run(async () =>
            {
                try
                {
                    // 创建新的作用域，避免线程安全问题
                    using var scope = scopeFactory.CreateScope();
                    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                    var apiAccessLog = new DataPersistence.EfCore.Entities.ApiAccessLog
                    {
                        SessionId = logSessionId,
                        UserId = logUserId,
                        HttpMethod = logHttpMethod,
                        ApiPath = logApiPath,
                        QueryString = logQueryString,
                        RequestBody = logRequestBody,
                        ResponseStatusCode = logStatusCode,
                        ResponseTimeMs = logResponseTimeMs,
                        IpAddress = logIpAddress,
                        UserAgent = logUserAgent,
                        ErrorMessage = logErrorMessage,
                        CreatedOn = logCreatedOn
                    };

                    dbContext.Set<DataPersistence.EfCore.Entities.ApiAccessLog>().Add(apiAccessLog);
                    await dbContext.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "写入 API 访问日志失败");
                }
            });
        }
    }

    private bool ShouldSkip(PathString path)
    {
        if (string.IsNullOrEmpty(path)) return true;

        return SkipPrefixes.Any(prefix =>
            path.StartsWithSegments(prefix, StringComparison.OrdinalIgnoreCase));
    }

    private bool HasRequestBody(HttpRequest request)
    {
        return request.ContentLength.HasValue &&
               request.ContentLength > 0 &&
               request.ContentLength <= 10240; // ≤ 10KB
    }

    private async Task<string?> CaptureRequestBodyAsync(HttpRequest request)
    {
        if (!request.Body.CanSeek)
        {
            return null;
        }

        request.Body.Position = 0;
        using var reader = new StreamReader(request.Body, leaveOpen: true);
        var body = await reader.ReadToEndAsync();
        request.Body.Position = 0;

        // 脱敏处理
        return SanitizeBody(body);
    }

    private string SanitizeBody(string body)
    {
        if (string.IsNullOrEmpty(body)) return body;

        // 移除敏感字段
        foreach (var field in SensitiveFields)
        {
            var pattern = $"(\"{field}\"\\s*:\\s*)\"[^\"]*\"";
            body = Regex.Replace(body, pattern, "$1\"***\"", RegexOptions.IgnoreCase);

            // 也处理没有引号的情况
            var pattern2 = "(\"" + field + "\"\\s*:\\s*)([^,}\\]]+)";
            body = Regex.Replace(body, pattern2, "$1\"***\"", RegexOptions.IgnoreCase);
        }

        // 截断过长的请求体
        return body.Length > 2000 ? body[..2000] + " [TRUNCATED]" : body;
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
}