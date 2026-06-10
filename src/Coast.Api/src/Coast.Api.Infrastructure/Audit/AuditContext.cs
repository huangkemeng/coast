namespace Coast.Api.Infrastructure.Audit;

/// <summary>
/// 当前请求的审计上下文
/// </summary>
public class AuditContext
{
    /// <summary>当前用户ID</summary>
    public Guid? UserId { get; set; }

    /// <summary>当前用户名</summary>
    public string? UserName { get; set; }

    /// <summary>会话ID</summary>
    public Guid? SessionId { get; set; }

    /// <summary>IP地址</summary>
    public string? IpAddress { get; set; }

    /// <summary>User-Agent</summary>
    public string? UserAgent { get; set; }

    /// <summary>当前HTTP请求路径</summary>
    public string? RequestPath { get; set; }

    /// <summary>操作时间</summary>
    public DateTime OperationTime { get; set; } = DateTime.UtcNow;
}