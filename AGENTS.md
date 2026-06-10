# Coast.Api 开发规范指南

> 本文件为 AI Coding 工具提供项目开发规范，确保 AI 生成的代码符合项目的架构约定和编码风格。

---

## 1. 项目概述

### 1.1 项目信息

| 属性 | 说明 |
|------|------|
| **项目名称** | Coast.Api - 需求跟踪管理系统 |
| **技术栈** | .NET 8 Web API |
| **架构模式** | Clean Architecture + CQRS |
| **依赖注入** | Autofac |
| **ORM** | EF Core + MongoDB |
| **认证方式** | JWT (bcrypt 加密) |
| **日志系统** | Seq |

### 1.2 解决方案结构

```
Coast.Api.sln
├── src/Coast.Api/src/
│   ├── Coast.Api/                    # Web API 主项目（入口）
│   ├── Coast.Api.Primary/            # 业务契约层（Interfaces/Contracts）
│   ├── Coast.Api.Infrastructure/     # 基础设施层（数据持久化、配置）
│   ├── Coast.Api.Realization/         # 业务实现层（Handlers）
│   ├── Coast.Api.Engines/             # 引擎配置层（AutoMapper、Mediator 等）
│   └── Coast.Api.DbMigration/         # 数据库迁移项目
├── src.tests/
│   ├── Coast.Api.UnitTests/           # 单元测试
│   └── Coast.Api.IntegrationTests/    # 集成测试
└── src.analyzers/
    ├── Coast.Api.CodeAnalyzers/       # 代码分析器
    └── Coast.Api.CodeGenerator/       # 代码生成器
```

---

## 2. 通用开发规则

### 2.1 必须遵守的规则

1. **编译验证**：每次修改代码后，必须运行 `dotnet build` 确保无编译错误
2. **错误修复**：若存在编译错误，根据错误信息修复后重新编译，直到成功
3. **默认语言**：代码注释使用中文，默认输出语言为中文
4. **编程语言**：除非另有说明，使用 C# 作为编程语言
5. **遵循架构**：严格按照分层架构规范编写代码

### 2.2 代码注释规范

```csharp
/// <summary>
/// 这是 XML 文档注释，用于公开 API
/// </summary>
/// <param name="paramName">参数说明</param>
/// <returns>返回值说明</returns>
public class MyClass
{
    // 这是普通单行注释

    /// <summary>
    /// 公开方法使用 XML 文档注释
    /// </summary>
    public void PublicMethod() { }

    // 私有字段使用普通注释
    private readonly string _connectionString;
}
```

---

## 3. API 开发流程

### 3.1 公开 API 编写步骤（5 步法）

1. **定义契约接口**（`Coast.Api.Primary/Contracts/`）
2. **定义 Request/Command 模型**
3. **实现处理器**（`Coast.Api.Realization/Handlers/`）
4. **创建控制器**（`Coast.Api/Controllers/`）
5. **运行测试验证**

### 3.2 文件命名规范

| 类型 | 命名规则 | 示例 |
|------|----------|------|
| 契约接口 | `I{功能名}Contract` | `IUserContract` |
| Command（无返回值） | `{功能名}Command` | `CreateUserCommand` |
| Command（有返回值） | `{功能名}Command` + `Response` | `LoginCommand` + `LoginResponse` |
| Request（查询） | `{功能名}Request` | `GetUserInfoRequest` |
| 响应模型 | `{功能名}Response` | `LoginResponse` |
| 处理器 | `{契约名}Handler` | `LoginHandler` |
| 控制器 | `{模块名}Controller` | `UserController` |
| 验证器 | `{Command名}Validator` | `CreateUserCommandValidator` |

---

## 4. 契约层规范（Coast.Api.Primary）

### 4.1 契约接口类型

```csharp
// 不带返回值的 Command → 继承 ICommandContract<TCommand>
public interface ICommandContract<TCommand> : IContract 
    where TCommand : class, new();

// 带返回值的 Command → 继承 ICommandContract<TCommand, TResponse>
public interface ICommandContract<TCommand, TResponse> : IContract
    where TCommand : class, new();

// 带返回值的 Request（查询）→ 继承 IRequestContract<TRequest, TResponse>
public interface IRequestContract<TRequest, TResponse> : IContract
    where TRequest : class, new();
```

### 4.2 契约定义示例

```csharp
// 文件位置：Coast.Api.Primary/Contracts/IUserContract.cs
namespace Coast.Api.Primary.Contracts;

public interface ILoginContract : ICommandContract<LoginCommand, LoginResponse>
{
}

/// <summary>
/// 登录命令
/// </summary>
public class LoginCommand
{
    /// <summary>
    /// 用户名
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// 密码
    /// </summary>
    public string Password { get; set; } = string.Empty;
}

/// <summary>
/// 登录响应
/// </summary>
public class LoginResponse
{
    /// <summary>
    /// 访问令牌
    /// </summary>
    public string AccessToken { get; set; } = string.Empty;

    /// <summary>
    /// 刷新令牌
    /// </summary>
    public string RefreshToken { get; set; } = string.Empty;
}
```

### 4.3 契约接口定义规则

| 规则 | 说明 |
|------|------|
| **定义位置** | `Coast.Api.Primary/Contracts/` 文件夹 |
| **继承关系** | 根据功能选择正确的基接口 |
| **泛型约束** | Command/Request 必须 `where T : class, new()` |
| **命名空间** | `Coast.Api.Primary.Contracts` |

---

## 5. 实现层规范（Coast.Api.Realization）

### 5.1 处理器结构

每个 Handler 必须实现三个方法：

```csharp
public class LoginHandler : ILoginContract
{
    /// <summary>
    /// 验证器配置（使用 FluentValidation）
    /// </summary>
    public void Validate(ContractValidator<LoginCommand> validator)
    {
        validator.RuleFor(e => e.Username)
            .NotEmpty()
            .WithMessage("用户名不能为空");

        validator.RuleFor(e => e.Password)
            .NotEmpty();
    }

    /// <summary>
    /// 业务逻辑处理
    /// </summary>
    public Task<LoginResponse> Handle(
        IReceiveContext<LoginCommand> context, 
        CancellationToken cancellationToken)
    {
        var command = context.Message;
        // 业务逻辑...
        return Task.FromResult(new LoginResponse { ... });
    }

    /// <summary>
    /// 测试用例定义
    /// </summary>
    public void Test(TestContext<LoginCommand, LoginResponse> context)
    {
        // 测试代码...
    }
}
```

### 5.2 处理器规则

| 规则 | 说明 |
|------|------|
| **实现接口** | 继承对应的 Contract 接口 |
| **验证方法** | `Validate` 方法配置 FluentValidation 规则 |
| **处理方法** | `Handle` 方法编写业务逻辑 |
| **测试方法** | `Test` 方法定义测试用例 |
| **无需 SaveChanges** | Mediator Pipeline 已处理事务 |
| **文件夹** | `Coast.Api.Realization/Handlers/{模块名}/` |

### 5.3 常用注入模式

```csharp
// 在 Handler 中注入依赖
public class UserHandler : IUserContract
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;

    public UserHandler(IUserRepository userRepository, IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }

    public Task<Response> Handle(IReceiveContext<Command> context, CancellationToken ct)
    {
        // 使用注入的服务
    }
}
```

---

## 6. 控制器规范（Coast.Api）

### 6.1 控制器定义

```csharp
// 文件位置：Coast.Api/Controllers/UserController.cs
public class UserController : BaseController
{
    /// <summary>
    /// 用户登录
    /// </summary>
    /// <param name="command">登录命令</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns></returns>
    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Login(
        [FromBody] LoginCommand command,
        CancellationToken cancellationToken)
    {
        var response = await Mediator.SendAsync<LoginCommand, LoginResponse>(
            command, cancellationToken);
        return Ok(response);
    }

    /// <summary>
    /// 获取用户信息
    /// </summary>
    /// <param name="request">查询请求</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns></returns>
    [HttpGet]
    [ProducesResponseType(typeof(GetUserInfoResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUserInfo(
        [FromQuery] GetUserInfoRequest request,
        CancellationToken cancellationToken)
    {
        var response = await Mediator.RequestAsync<GetUserInfoRequest, GetUserInfoResponse>(
            request, cancellationToken);
        return Ok(response);
    }
}
```

### 6.2 控制器规则

| 规则 | 说明 |
|------|------|
| **基类** | 继承 `BaseController`（位于 `Coast.Api.Controllers.Bases`） |
| **命名** | `{模块名}Controller`，如 `UserController` |
| **方法名** | `{方法名}Async`，如 `LoginAsync` |
| **返回类型** | `Task<IActionResult>` |
| **参数来源** | 使用 `[FromBody]`、`[FromQuery]`、`[FromRoute]` 标记 |
| **必填注解** | 配置 `[ProducesResponseType<T>(200)]` |

### 6.3 HTTP 方法对应关系

| 操作 | HTTP 方法 | Mediator 调用 |
|------|-----------|---------------|
| Create | `[HttpPost]` | `SendAsync<Command, Response>` |
| Read | `[HttpGet]` | `RequestAsync<Request, Response>` |
| Update | `[HttpPut]` | `SendAsync<Command, Response>` |
| Delete | `[HttpDelete]` | `SendAsync<Command>` |

---

## 7. RESTful API 设计规范

### 7.1 核心原则

| 原则 | 说明 |
|------|------|
| **资源导向** | URL 表示资源，不是动作 |
| **统一接口** | 使用标准 HTTP 方法表达 CRUD |
| **无状态** | 每个请求包含所有必要信息 |
| **分层系统** | 客户端不需要知道后端实现细节 |

### 7.2 URL 命名规范

| 资源类型 | URL 模式 | 示例 |
|----------|----------|------|
| 集合资源 | `/resources` | `/users`, `/requirements` |
| 单个资源 | `/resources/{id}` | `/users/123`, `/requirements/456` |
| 嵌套资源 | `/resources/{id}/sub-resources` | `/users/123/assignments` |
| 动作（RPC 风格） | `/resources/{id}/actions` | `/users/123/reset-password` |

**必须遵守的规则**：

```csharp
// ✅ 正确：使用复数名词、kebab-case
[HttpGet("user-accounts")]
[HttpGet("requirements/{id}")]

// ❌ 错误：使用动词、单数名词、camelCase
[HttpGet("getUsers")]
[HttpGet("getUserById")]
```

| 规则 | 说明 | 示例 |
|------|------|------|
| 使用复数名词 | 资源名称用复数 | `/users` 而非 `/user` |
| 使用 kebab-case | 小写 + 连字符 | `/user-accounts` |
| 避免动词 | HTTP 方法即动作 | `GET /users/123` |
| 嵌套限制 | 最多 2 层嵌套 | `/users/123/roles` |

### 7.3 HTTP 方法使用规范

| 方法 | 用途 | URL 示例 | 响应状态码 |
|------|------|----------|-------------|
| `GET` | 查询资源列表 | `GET /users` | 200 + 分页列表 |
| `GET` | 查询单个资源 | `GET /users/123` | 200 + 资源 |
| `POST` | 创建资源 | `POST /users` | 201 + 资源 |
| `PUT` | 完整更新资源 | `PUT /users/123` | 200 + 资源 |
| `PATCH` | 部分更新资源 | `PATCH /users/123` | 200 + 资源 |
| `DELETE` | 删除资源 | `DELETE /users/123` | 204（无内容） |

**控制器方法命名规范**：

```csharp
// ✅ 正确：使用 HTTP 方法对应的动词前缀
[HttpGet]
public async Task<IActionResult> GetUsersAsync() { }

[HttpPost]
public async Task<IActionResult> CreateUserAsync() { }

[HttpPut("{id:long}")]
public async Task<IActionResult> UpdateUserAsync() { }

[HttpDelete("{id:long}")]
public async Task<IActionResult> DeleteUserAsync() { }

// ❌ 错误：使用 CRUD 动词
[HttpGet]
public async Task<IActionResult> QueryUsersAsync() { }  // 用 Get

[HttpPost]
public async Task<IActionResult> AddUserAsync() { }      // 用 Create
```

### 7.4 HTTP 状态码使用指南

| 状态码 | 含义 | 使用场景 |
|--------|------|----------|
| **2xx 成功** | | |
| 200 | OK | 查询成功、更新成功 |
| 201 | Created | POST 创建新资源 |
| 204 | No Content | DELETE 成功，无返回内容 |
| **4xx 客户端错误** | | |
| 400 | Bad Request | 参数校验失败、格式错误 |
| 401 | Unauthorized | 未提供或无效的 Token |
| 403 | Forbidden | 用户无权限执行操作 |
| 404 | Not Found | 资源不存在 |
| 409 | Conflict | 资源状态冲突（并发问题） |
| 422 | Unprocessable Entity | 业务规则校验失败 |
| **5xx 服务器错误** | | |
| 500 | Internal Server Error | 系统异常 |
| 503 | Service Unavailable | 服务不可用 |

### 7.5 错误响应格式

**标准错误响应结构**：

```csharp
public class ErrorResponse
{
    /// <summary>
    /// 错误代码（业务码）
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// 用户友好的错误消息
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// 详细错误信息（可选）
    /// </summary>
    public List<ErrorDetail>? Details { get; set; }

    /// <summary>
    /// 请求追踪 ID（用于日志关联）
    /// </summary>
    public string? TraceId { get; set; }
}

public class ErrorDetail
{
    /// <summary>
    /// 错误字段（用于表单验证）
    /// </summary>
    public string? Field { get; set; }

    /// <summary>
    /// 字段级错误消息
    /// </summary>
    public string Error { get; set; } = string.Empty;
}
```

**JSON 响应示例**：

```json
// 400 - 参数验证失败
{
  "code": "VALIDATION_ERROR",
  "message": "请求参数校验失败",
  "details": [
    { "field": "username", "error": "用户名不能为空" },
    { "field": "email", "error": "邮箱格式不正确" }
  ],
  "traceId": "abc123-def456"
}

// 404 - 资源不存在
{
  "code": "USER_NOT_FOUND",
  "message": "指定的用户不存在",
  "traceId": "abc123-def456"
}

// 422 - 业务规则错误
{
  "code": "REQUIREMENT_STATUS_INVALID",
  "message": "需求状态不允许此操作",
  "details": [
    { "field": "status", "error": "当前状态为『已上线』，不允许回退" }
  ],
  "traceId": "abc123-def456"
}
```

### 7.6 成功响应格式

**单资源响应**：

```json
{
  "id": 123,
  "username": "admin",
  "email": "admin@example.com",
  "createdAt": "2026-01-01T00:00:00Z"
}
```

**分页列表响应**：

```json
{
  "items": [
    { "id": 1, "username": "user1" },
    { "id": 2, "username": "user2" }
  ],
  "totalCount": 100,
  "page": 1,
  "pageSize": 20,
  "totalPages": 5
}
```

**创建成功响应（201）**：

```json
{
  "id": 123,
  "username": "newuser",
  "createdAt": "2026-01-01T00:00:00Z"
}
```

### 7.7 查询参数规范

| 参数 | 用途 | 示例 |
|------|------|------|
| `page` | 页码 | `GET /users?page=2` |
| `pageSize` | 每页数量 | `GET /users?pageSize=20` |
| `sort` | 排序字段 | `GET /users?sort=createdAt` |
| `order` | 排序方向 | `GET /users?order=desc` |
| `search` | 搜索关键字 | `GET /users?search=keyword` |
| `{field}` | 精确过滤 | `GET /requirements?status=open` |

```csharp
// ✅ 正确：查询列表带分页
[HttpGet]
public async Task<IActionResult> GetRequirementsAsync(
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 20,
    [FromQuery] string? status = null,
    CancellationToken cancellationToken = default)
{
    var request = new GetRequirementsRequest
    {
        Page = page,
        PageSize = pageSize,
        Status = status
    };
    var response = await Mediator.RequestAsync(request, cancellationToken);
    return Ok(response);
}
```

### 7.8 RESTful 控制器完整示例

```csharp
/// <summary>
/// 用户管理
/// </summary>
[ApiController]
[Route("users")]
public class UserController : BaseController
{
    /// <summary>
    /// 获取用户列表（分页）
    /// </summary>
    /// <param name="request">查询参数</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>用户列表</returns>
    [HttpGet]
    [ProducesResponseType(typeof(PaginatedResponse<UserDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUsersAsync(
        [FromQuery] GetUsersRequest request,
        CancellationToken cancellationToken)
    {
        var response = await Mediator.RequestAsync(request, cancellationToken);
        return Ok(response);
    }

    /// <summary>
    /// 获取单个用户
    /// </summary>
    /// <param name="id">用户 ID</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>用户信息</returns>
    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUserAsync(
        long id,
        CancellationToken cancellationToken)
    {
        var request = new GetUserRequest { Id = id };
        var response = await Mediator.RequestAsync(request, cancellationToken);
        return Ok(response);
    }

    /// <summary>
    /// 创建用户
    /// </summary>
    /// <param name="command">创建用户命令</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>创建的用户</returns>
    [HttpPost]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> CreateUserAsync(
        [FromBody] CreateUserCommand command,
        CancellationToken cancellationToken)
    {
        var response = await Mediator.SendAsync(command, cancellationToken);
        return CreatedAtAction(
            nameof(GetUserAsync),
            new { id = response.Id },
            response);
    }

    /// <summary>
    /// 更新用户
    /// </summary>
    /// <param name="id">用户 ID</param>
    /// <param name="command">更新用户命令</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>更新后的用户</returns>
    [HttpPut("{id:long}")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateUserAsync(
        long id,
        [FromBody] UpdateUserCommand command,
        CancellationToken cancellationToken)
    {
        command.Id = id;
        var response = await Mediator.SendAsync(command, cancellationToken);
        return Ok(response);
    }

    /// <summary>
    /// 删除用户
    /// </summary>
    /// <param name="id">用户 ID</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>无内容</returns>
    [HttpDelete("{id:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteUserAsync(
        long id,
        CancellationToken cancellationToken)
    {
        var command = new DeleteUserCommand { Id = id };
        await Mediator.SendAsync(command, cancellationToken);
        return NoContent();
    }
}
```

### 7.9 RESTful 禁止事项

```csharp
// ❌ 禁止：在 URL 中使用动词
[HttpGet("getUserById/{id}")]        // 用 GET /users/{id}
[HttpPost("createUser")]             // 用 POST /users
[HttpPost("login")]                  // 用 POST /auth/login 或 POST /sessions

// ❌ 禁止：使用非 RESTful 的动作路由
[HttpPost("users/{id}/activate")]    // 用 PATCH /users/{id} + { "isActive": true }
[HttpPost("users/{id}/deactivate")]  // 用 PATCH /users/{id} + { "isActive": false }

// ❌ 禁止：嵌套过深（超过 2 层）
[HttpGet("orgs/{orgId}/teams/{teamId}/members/{memberId}/roles")]
// 改为：直接用 /team-members/{id} 或 /members/{id}/team-roles

// ❌ 禁止：使用不正确的 HTTP 方法
[HttpGet]                             // 永远不要用 GET 做删除
public async Task<IActionResult> DeleteUserAsync()

// ❌ 禁止：使用错误的响应状态码
return Ok();                          // POST 创建成功后用 CreatedAtAction
return Ok();                          // DELETE 成功后用 NoContent
```

---

## 8. 数据实体规范（Coast.Api.Infrastructure）

### 7.1 实体基类

```csharp
// 所有数据实体都应实现 IEntity 接口
public interface IEntity
{
}

// 扩展接口
public interface ICanSoftDelete { }  // 支持软删除
public interface IHasCreatedOn { DateTime CreatedOn { get; set; } }  // 创建时间
public interface IHasCreator<TKey> { TKey CreatorId { get; set; } }  // 创建人
public interface IHasUpdater<TKey> { TKey UpdaterId { get; set; } }  // 更新人
public interface IExtendedEntity : IEntity { }  // 扩展实体
```

### 7.2 实体命名规范

| 规则 | 示例 |
|------|------|
| 表名 | `PascalCase`，如 `UserAccount` |
| 主键 | `Id` 或 `{表名}Id`，类型根据数据库 |
| 外键 | `{相关表名}Id`，如 `UserId` |
| 布尔字段 | `Is{状态}`，`Has{属性}`，如 `IsDeleted`、`HasPermission` |

---

## 8. 测试规范

### 8.1 测试框架

| 组件 | 框架 |
|------|------|
| 测试框架 | xUnit |
| 断言库 | Shouldly |
| 验证库 | FluentValidation |
| Mock | Moq 或 NSubstitute |

### 8.2 Handler 内置测试

```csharp
public void Test(TestContext<LoginCommand, LoginResponse> context)
{
    // 不需要数据库的测试
    context.NoDatabase = true;

    // 创建测试用例
    var loginSuccessfullyCase = context.CreateTestCase();
    loginSuccessfullyCase.Build = builder =>
    {
        // 配置 Mock
    };
    loginSuccessfullyCase.Message = new LoginCommand
    {
        Username = "admin",
        Password = "Admin@123"
    };
    loginSuccessfullyCase.Arrange = async () =>
    {
        // 准备测试数据
        await Task.CompletedTask;
    };
    loginSuccessfullyCase.Assert = result =>
    {
        result.Exception.ShouldBeNull();
        result.Response.AccessToken.ShouldNotBeNullOrWhiteSpace();
        return Task.CompletedTask;
    };
}
```

### 8.3 测试文件位置

```
src.tests/
├── Coast.Api.UnitTests/
│   └── Handlers/
│       └── LoginHandlerTests.cs
└── Coast.Api.IntegrationTests/
    └── Handlers/
        └── LoginHandlerIntegrationTests.cs
```

---

## 9. 业务规则要点（来自 PRD）

### 9.1 需求状态流转

| 状态 | 可流转至 | 说明 |
|------|----------|------|
| 待确认 | 已确认 | 需求管理员确认 |
| 已确认 | 待报价 | 发起报价流程 |
| 待报价 | 已报价 | 填写报价 |
| 已报价 | 待开发 | 确认报价 |
| 待开发 | 开发中 | 开始处理 |
| 开发中 | 测试中 | 提交测试 |
| 测试中 | 已验收待上线 | 测试通过 |
| 已验收待上线 | 已上线 | 确认上线 |
| **已上线** | **-** | **终态，不可流转** |

**规则**：
- 仅允许前后相邻状态流转，禁止跨状态跳转
- "已上线"为终态，不可回退

### 9.2 用户角色权限

| 角色 | 权限 |
|------|------|
| 管理员 | 全部权限 |
| 开发人员 | 查看/更新分配给自己的需求 |
| 测试人员 | 查看/更新分配给自己的需求 |

### 9.3 认证安全策略

| 规则 | 说明 |
|------|------|
| 密码加密 | bcrypt，强度12 |
| 密码规则 | 至少8位，包含大小写字母和数字 |
| 错误限制 | 连续错误5次，锁定30分钟 |
| JWT 有效期 | Access Token 2小时，Refresh Token 7天 |

---

## 10. 配置位置索引

| 功能 | 配置位置 |
|------|----------|
| JWT 配置 | `Coast.Api.Infrastructure/JwtFunction/` |
| CORS 配置 | `Coast.Api.Infrastructure/CorsFunction/` |
| EF Core | `Coast.Api.Infrastructure/DataPersistence/EfCore/` |
| MongoDB | `Coast.Api.Infrastructure/DataPersistence/MongoDb/` |
| AutoMapper | `Coast.Api.Engines/AutoMapperEngines/` |
| Mediator | `Coast.Api.Engines/MediatorEngines/` |
| Swagger | `Coast.Api.Engines/SwaggerEngines/` |
| 日志 (Seq) | `Coast.Api.Infrastructure/SeqLog/` |

---

## 11. 常见问题处理

### 11.1 并发控制（乐观锁）

```csharp
// 实体中添加版本号字段
public class Requirement : IEntity
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int Version { get; set; }  // 并发控制版本号
}

// 更新时检查版本
if (entity.Version != expectedVersion)
{
    throw new ConcurrencyException("数据已被他人修改，请刷新页面");
}
entity.Version++;  // 版本号自增
```

### 11.2 软删除实现

```csharp
// 实体实现 ICanSoftDelete
public class User : IEntity, ICanSoftDelete
{
    public bool IsDeleted { get; set; }
}

// 查询时自动过滤
public IEnumerable<T> GetAll<T>() where T : ICanSoftDelete
{
    return _dbContext.Set<T>().Where(x => !x.IsDeleted);
}
```

### 11.3 分页查询

```csharp
// 使用 IPageable 接口
public interface IPageable
{
    int Page { get; set; }
    int PageSize { get; set; }
}

// 响应使用 IPaginated 接口
public interface IPaginated<T>
{
    int TotalCount { get; set; }
    int TotalPages { get; set; }
    IEnumerable<T> Items { get; set; }
}
```

---

## 12. 禁止事项

❌ **禁止** 在 Handler 中直接调用 `SaveChanges()`，Mediator Pipeline 已处理

❌ **禁止** 跨层直接依赖（下层不能引用上层）

❌ **禁止** 在控制器中写业务逻辑

❌ **禁止** 使用 `var` 声明具有业务含义的变量

❌ **禁止** 硬编码配置值，应使用配置类或环境变量

❌ **禁止** 在循环中执行数据库查询（N+1 问题）

---

## 13. 参考示例

| 示例 | 路径 |
|------|------|
| Handler 示例 | [LoginHandler.cs](src/Coast.Api/src/Coast.Api.Realization/Handlers/LoginHandler.cs) |
| Controller 示例 | [UserController.cs](src/Coast.Api/src/Coast.Api/Controllers/UserController.cs) |
| 契约接口示例 | [ICommandContract.cs](src/Coast.Api/src/Coast.Api.Primary/Contracts/Bases/ICommandContract.cs) |
| 数据实体基类 | [IEntity.cs](src/Coast.Api/src/Coast.Api.Infrastructure/DataPersistence/DataEntityBases/IEntity.cs) |
| 测试基类 | [TestBase.cs](src/Coast.Api/src.tests/Coast.Api.UnitTests/TestBase.cs) |

---

**文档版本**: v1.0  
**最后更新**: 2026-06-10  
**维护者**: AI Coding Team
