using Coast.Api.Controllers.Bases;
using Coast.Api.Primary.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace Coast.Api.Controllers;

/// <summary>
/// 用户管理
/// </summary>
[ApiController]
[Route("users")]
public class UserController : BaseController
{
    /// <summary>
    /// 用户登录（创建会话）
    /// </summary>
    /// <param name="command">登录凭证</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>创建的会话信息</returns>
    /// <response code="201">登录成功</response>
    /// <response code="401">用户名或密码错误</response>
    /// <response code="422">登录受限（如账号被锁定）</response>
    [HttpPost("~/sessions")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> LoginAsync(
        [FromBody] LoginCommand command,
        CancellationToken cancellationToken)
    {
        var response = await Mediator.SendAsync<LoginCommand, LoginResponse>(command, cancellationToken);
        // 返回 201 Created，Location 指向当前用户资源
        return Created($"/users/{command.Username}", response);
    }

    /// <summary>
    /// 获取当前登录用户信息
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>当前用户信息</returns>
    /// <response code="200">成功返回用户信息</response>
    [HttpGet("me")]
    [ProducesResponseType(typeof(GetUserInfoResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCurrentUserAsync(CancellationToken cancellationToken)
    {
        // TODO: 从 ClaimsPrincipal 获取当前用户名
        var request = new GetUserInfoRequest { Username = User.Identity?.Name ?? string.Empty };
        var response = await Mediator.RequestAsync<GetUserInfoRequest, GetUserInfoResponse>(request, cancellationToken);
        return Ok(response);
    }

    /// <summary>
    /// 通过用户名获取用户信息
    /// </summary>
    /// <param name="username">用户名</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>用户信息</returns>
    /// <response code="200">成功返回用户信息</response>
    /// <response code="404">用户不存在</response>
    [HttpGet("{username}")]
    [ProducesResponseType(typeof(GetUserInfoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUserAsync(
        string username,
        CancellationToken cancellationToken)
    {
        var request = new GetUserInfoRequest { Username = username };
        var response = await Mediator.RequestAsync<GetUserInfoRequest, GetUserInfoResponse>(request, cancellationToken);
        return Ok(response);
    }
}

/// <summary>
/// 错误响应
/// </summary>
public class ErrorResponse
{
    /// <summary>
    /// 错误代码
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// 错误消息
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// 详细错误信息
    /// </summary>
    public List<ErrorDetail>? Details { get; set; }

    /// <summary>
    /// 请求追踪 ID
    /// </summary>
    public string? TraceId { get; set; }
}

/// <summary>
/// 错误详情
/// </summary>
public class ErrorDetail
{
    /// <summary>
    /// 错误字段
    /// </summary>
    public string? Field { get; set; }

    /// <summary>
    /// 字段级错误消息
    /// </summary>
    public string Error { get; set; } = string.Empty;
}
