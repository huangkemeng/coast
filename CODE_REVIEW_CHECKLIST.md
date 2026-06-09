# 代码审查检查清单

> 本清单用于 AI Coding 编写代码后的自我审查，确保代码符合项目规范。

---

## 1. 架构规范检查

### 1.1 Clean Architecture 分层
- [ ] **契约层**（Coast.Api.Primary）只定义接口和模型，不包含业务逻辑
- [ ] **实现层**（Coast.Api.Realization）实现契约接口，包含业务逻辑
- [ ] **控制器层**（Coast.Api）只负责 HTTP 请求处理，不包含业务逻辑
- [ ] **基础设施层**（Coast.Api.Infrastructure）提供数据持久化、配置等基础设施

### 1.2 依赖关系
- [ ] 上层不直接引用下层（控制器 → 契约 → 实现）
- [ ] 实现层不直接引用控制器层
- [ ] 使用依赖注入而非直接实例化

### 1.3 CQRS 模式
- [ ] 命令（Command）使用 `ICommandContract<TCommand, TResponse>` 或 `ICommandContract<TCommand>`
- [ ] 查询（Request）使用 `IRequestContract<TRequest, TResponse>`
- [ ] 命名遵循 `{功能名}Command` / `{功能名}Request` / `{功能名}Response` 模式

---

## 2.契约层检查（Contracts）

### 2.1 接口定义
- [ ] 命名遵循 `I{功能名}Contract` 模式
- [ ] 继承正确的基接口（`ICommandContract` / `IRequestContract`）
- [ ] Command/Request 泛型约束 `where T : class, new()`

### 2.2 模型定义
- [ ] 使用 `[Required]` 等数据注解标记必填字段
- [ ] 字段命名使用 PascalCase
- [ ] 包含中文 XML 文档注释
- [ ] 提供默认值初始化

### 2.3 示例
```csharp
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
```

---

## 3. 实现层检查（Handlers）

### 3.1 结构完整性
- [ ]继承对应的 Contract 接口
- [ ] 实现 `Validate` 方法配置验证规则
- [ ] 实现 `Handle` 方法编写业务逻辑
- [ ] 实现 `Test` 方法定义测试用例

### 3.2 业务逻辑
- [ ] Handle 方法内不调用 `SaveChanges()`（由 Mediator Pipeline 处理）
- [ ] 使用注入的服务进行数据操作
- [ ] 正确处理异常情况
- [ ] 返回值类型与契约一致

### 3.3 验证规则
- [ ] 使用 FluentValidation 编写验证规则
- [ ] 验证消息使用中文
- [ ] 覆盖所有边界情况

### 3.4 示例
```csharp
public class LoginHandler : ILoginContract
{
    public void Validate(ContractValidator<LoginCommand> validator)
    {
        validator.RuleFor(e => e.Username)
            .NotEmpty()
            .WithMessage("用户名不能为空");

        validator.RuleFor(e => e.Password)
            .NotEmpty()
            .WithMessage("密码不能为空");
    }

    public Task<LoginResponse> Handle(
        IReceiveContext<LoginCommand> context, 
        CancellationToken cancellationToken)
    {
        var command = context.Message;
        // 业务逻辑...
        return Task.FromResult(new LoginResponse { ... });
    }

    public void Test(TestContext<LoginCommand, LoginResponse> context)
    {
        context.NoDatabase = true;
        // 测试用例...
    }
}
```

---

## 4. 控制器检查（Controllers）

### 4.1 基类继承
- [ ] 继承 `WebBaseController`（非 `BaseController` 或其他）
- [ ] 使用 `IMediator` 发送请求

### 4.2 方法定义
- [ ] 方法名遵循 `{方法名}Async` 模式
- [ ] 返回类型为 `Task<IActionResult>`
- [ ] 参数使用 `[FromBody]`、`[FromQuery]`、`[FromRoute]` 标记来源

### 4.3 API 配置
- [ ] 配置 `[ProducesResponseType<T>(200)]`
- [ ] 配置正确的 HTTP 方法属性（`[HttpPost]`、`[HttpGet]` 等）
- [ ] Route 使用 kebab-case（如 `login`、`get-user`）

### 4.4 示例
```csharp
public class UserController : WebBaseController
{
    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> LoginAsync(
        [FromBody] LoginCommand command,
        CancellationToken cancellationToken)
    {
        var response = await Mediator.SendAsync<LoginCommand, LoginResponse>(
            command, cancellationToken);
        return Ok(response);
    }
}
```

---

## 5. 数据实体检查

### 5.1 基类实现
- [ ] 实现 `IEntity` 接口
- [ ] 需要软删除时实现 `ICanSoftDelete`
- [ ] 需要审计字段时实现 `IHasCreatedOn`、`IHasUpdatedOn` 等

### 5.2 字段定义
- [ ] 主键命名为 `Id` 或 `{表名}Id`
- [ ] 外键命名为 `{相关表名}Id`
- [ ] 布尔字段使用 `Is`/`Has` 前缀

### 5.3 EF Core 配置
- [ ] 在 `ConfigureEntityMapping` 中配置实体映射
- [ ] 配置必要的索引
- [ ] 配置级联删除规则

---

## 6. 通用代码质量

### 6.1 命名规范
- [ ] 类名、方法名使用 PascalCase
- [ ] 私有字段使用 `_camelCase` 下划线前缀
- [ ] 常量使用 PascalCase
- [ ] 接口名以 `I` 开头

### 6.2 注释规范
- [ ] 公开类型和方法使用 XML 文档注释
- [ ] 复杂业务逻辑添加行内注释
- [ ] 注释使用中文

### 6.3 错误处理
- [ ]验证失败时抛出 `ValidationException` 或返回错误响应
- [ ] 业务异常使用自定义异常类型
- [ ] 未找到资源时返回404

### 6.4 性能考虑
- [ ] 避免 N+1 查询问题
- [ ] 使用 `AsNoTracking()` 进行只读查询
- [ ] 大数据量分页处理

---

## 7. 业务规则检查

### 7.1 需求管理
- [ ] 状态流转仅允许相邻状态变更
- [ ] "已上线"状态不可回退
- [ ] 报价字段仅管理员可见
- [ ] 并发控制使用版本号

### 7.2 权限控制
- [ ] 敏感操作需要管理员权限
- [ ] 用户只能操作自己有权限的数据
- [ ]验证用户身份

### 7.3 数据验证
- [ ] URL 字段校验格式（http/https，内网地址）
- [ ] 时间字段逻辑校验（交测时间不早于开始时间）
- [ ] 数值字段范围校验（进度 0-100，报价 ≥0）

---

## 8. 测试检查

### 8.1 单元测试
- [ ] 每个 Handler 有对应的测试
- [ ] 测试覆盖正常流程和异常流程
- [ ] 使用 `context.NoDatabase = true` 标记无数据库测试

### 8.2 测试命名
- [ ] 测试方法名清晰描述测试场景
- [ ] 使用 Arrange-Act-Assert 模式

---

## 9. 提交前检查

- [ ] 代码已格式化（Ctrl+Shift+F）
- [ ] 无编译错误
- [ ] 所有测试通过
- [ ] 代码已添加到版本控制
- [ ] 提交信息符合规范（Conventional Commits）

---

## 常见问题快速修复

| 问题 | 解决方案 |
|------|----------|
| 忘记添加 using | 运行 `dotnet build` 查看缺失引用 |
| Handler 没实现接口 | 实现完整的 Validate/Handle/Test |
| 控制器没继承基类 | 改为继承 `WebBaseController` |
| 忘记配置 ProducesResponseType | 添加 `[ProducesResponseType]` 属性 |
| 硬编码值 | 移至配置文件或常量类 |
| 使用 var声明业务变量 | 改用具体类型 |

---

**维护者**: AI Coding Team  
**版本**: v1.0  
**最后更新**: 2026-06-10