# RequirementTrackingSystem 项目规则

## 1. 通用规则

- 修改项目文件后，必须编译解决方案确保无错误
- 编译出现错误时，根据错误提示修复后再次编译直到成功
- 默认使用中文作为输出语言，默认添加代码注释
- 如无特殊说明，使用 C# 作为编程语言

## 2. 项目结构

项目基于 ContractFirst API 模板，采用分层架构，结构如下：

```
RequirementTrackingSystem/
├── src/
│   ├── RequirementTrackingSystem/                      # Web API 主项目
│   │   ├── Controllers/                                # API 控制器
│   │   │   └── Bases/                                  # 控制器基类
│   │   ├── FilterAndMiddlewares/                       # 过滤器和中间件
│   │   ├── Properties/                                # 启动配置
│   │   └── Program.cs                                 # 入口文件
│   │
│   ├── RequirementTrackingSystem.Primary/              # 业务契约层（接口定义）
│   │   ├── Bases/                                     # 基础类
│   │   └── Contracts/                                 # 契约接口
│   │       └── Bases/                                 # 契约基类
│   │
│   ├── RequirementTrackingSystem.Realization/           # 业务实现层（处理逻辑）
│   │   ├── Bases/                                     # 基础类
│   │   ├── Currents/                                  # 当前用户上下文
│   │   └── Handlers/                                  # 处理器实现
│   │       └── {ModuleName}/                          # 按模块组织的处理器
│   │
│   ├── RequirementTrackingSystem.Infrastructure/       # 基础设施层
│   │   ├── Bases/                                     # 基础类
│   │   ├── CorsFunction/                              # CORS 配置
│   │   ├── DataPersistence/                           # 数据持久化（EF Core）
│   │   ├── JwtFunction/                              # JWT 功能
│   │   └── SeqLog/                                   # 日志配置
│   │
│   ├── RequirementTrackingSystem.Engines/             # 引擎配置层
│   │   ├── AutoMapperEngines/                         # AutoMapper 配置
│   │   ├── EfCoreEngines/                            # EF Core 配置
│   │   ├── MediatorEngines/                          # Mediator 配置
│   │   └── SwaggerEngines/                           # Swagger 配置
│   │
│   └── RequirementTrackingSystem.DbMigration/         # 数据库迁移项目
│
├── src.tests/                                         # 测试项目
│   ├── RequirementTrackingSystem.UnitTests/           # 单元测试
│   └── RequirementTrackingSystem.IntegrationTests/    # 集成测试
│
└── src.analyzers/                                    # 代码分析器
```

## 3. 公开 API 编写步骤与文件

### 标准流程

1. **定义合约接口** (`Contract.cs`)
   - 在 `RequirementTrackingSystem.Primary/Contracts/` 下创建
   - 继承相应的基接口：`ICommandContract`、`IRequestContract`

2. **定义请求/命令模型**
   - 继承 `IMapFrom<TSource>` 用于字段映射
   - 使用 `[FromRoute]`、`[FromBody]` 等标记参数来源

3. **实现处理器** (`Handler.cs`)
   - 在 `RequirementTrackingSystem.Realization/Handlers/` 下创建
   - 实现三个方法：`Handle`、`Validate`、`Test`

4. **创建控制器** (`Controller.cs`)
   - 在 `RequirementTrackingSystem/Controllers/` 下创建
   - 使用 Mediator 发送命令/请求

5. **配置 API 端点**
   - 使用 `[ProducesResponseType<TResponse>(200)]`
   - 配置 HTTP 方法属性和路由

## 4. 控制器定义规范

- 文件位置：`src/RequirementTrackingSystem/Controllers/`
- 命名规则：`{模块名}Controller`，如 `UserController`、`RequirementController`
- 基类：继承 `WebBaseController`
- 使用 `IMediator` 发送命令和请求

### 示例

```csharp
public class RequirementController : WebBaseController
{
    [HttpPost]
    [ProducesResponseType(typeof(CreateRequirementResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateAsync(
        [FromBody] CreateRequirementCommand command,
        CancellationToken cancellationToken)
    {
        var response = await Mediator.SendAsync<CreateRequirementCommand, CreateRequirementResponse>(
            command, cancellationToken);
        return Ok(response);
    }
}
```

## 5. 合约接口定义规范

- 文件位置：`src/RequirementTrackingSystem.Primary/Contracts/`
- 命令有返回值 → 继承 `ICommandContract<TCommand, TResponse>`
- 命令无返回值 → 继承 `ICommandContract<TCommand>`
- 请求有返回值 → 继承 `IRequestContract<TRequest, TResponse>`
- 实现 `IMapFrom<TEntity>` 用于字段映射到实体

### 示例

```csharp
// 带返回值的命令
public interface ICreateRequirementContract : ICommandContract<CreateRequirementCommand, CreateRequirementResponse>
{
}

// 不带返回值的命令
public interface IDeleteRequirementContract : ICommandContract<DeleteRequirementCommand>
{
}

// 带返回值的请求
public interface IGetRequirementsContract : IRequestContract<GetRequirementsRequest, GetRequirementsResponse>
{
}
```

## 6. 处理器定义规范

- 文件位置：`src/RequirementTrackingSystem.Realization/Handlers/{模块名}/`
- 实现相应的合约接口
- 必须实现三个方法：`Handle`、`Validate`、`Test`

### 示例

```csharp
public class CreateRequirementHandler : ICreateRequirementContract
{
    private readonly ApplicationDbContext _dbContext;

    public CreateRequirementHandler(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void Validate(ContractValidator<CreateRequirementCommand> validator)
    {
        validator.RuleFor(e => e.Name).NotEmpty().MaximumLength(100);
        validator.RuleFor(e => e.RequirementNo).NotEmpty().MaximumLength(50);
    }

    public async Task<CreateRequirementResponse> Handle(
        IReceiveContext<CreateRequirementCommand> context,
        CancellationToken cancellationToken)
    {
        var command = context.Message;
        var requirement = new Requirement
        {
            Name = command.Name,
            RequirementNo = command.RequirementNo,
            Status = RequirementStatus.PendingConfirm,
            Version = 1,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _dbContext.Requirements.AddAsync(requirement, cancellationToken);
        // 无需调用 SaveChanges，Mediator Pipeline 已处理事务

        return new CreateRequirementResponse { Id = requirement.Id };
    }

    public void Test(TestContext<CreateRequirementCommand, CreateRequirementResponse> context)
    {
        context.NoDatabase = true; // 可选：设置为无需数据库
    }
}
```

## 7. 领域实体定义

- 文件位置：`src/RequirementTrackingSystem.Infrastructure/DataPersistence/EfCore/Entities/`
- 实体需实现 `IHasKey<TKey>` 接口
- 使用 Fluent API 配置实体映射

### 示例

```csharp
public class Requirement : IHasKey<int>
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string RequirementNo { get; set; }
    public RequirementStatus Status { get; set; }
    public int Progress { get; set; }
    public int FollowerId { get; set; }
    public User Follower { get; set; }
    public DateTime? PlanStartDate { get; set; }
    public DateTime? PlanTestDate { get; set; }
    public DateTime? PlanLaunchDate { get; set; }
    public DateTime? ActualTestDate { get; set; }
    public DateTime? ActualLaunchDate { get; set; }
    public bool IsConfirmed { get; set; }
    public string? DocUrl { get; set; }
    public decimal? Price { get; set; }
    public int ProjectId { get; set; }
    public Project Project { get; set; }
    public int? RobotId { get; set; }
    public Robot? Robot { get; set; }
    public Priority Priority { get; set; }
    public string? Remark { get; set; }
    public int Version { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public enum RequirementStatus
{
    PendingConfirm = 0,
    Confirmed = 1,
    PendingQuote = 2,
    Quoted = 3,
    PendingDev = 4,
    InDev = 5,
    InTest = 6,
    AcceptedPendingLaunch = 7,
    Launched = 8
}

public enum Priority
{
    Low = 0,
    Medium = 1,
    High = 2
}
```

## 8. 枚举定义规范

枚举值使用整数类型，遵循以下命名约定：
- 使用 PascalCase 命名枚举成员
- 使用有意义的值名称
- 终态（如 `Launched`）的数组应为空

### 状态流转规则

| 当前状态 | 允许的下一状态 |
|---------|--------------|
| PendingConfirm | Confirmed |
| Confirmed | PendingQuote |
| PendingQuote | Quoted |
| Quoted | PendingDev |
| PendingDev | InDev |
| InDev | InTest |
| InTest | AcceptedPendingLaunch |
| AcceptedPendingLaunch | Launched |
| Launched | （终态，无流转） |

## 9. DTO 定义规范

- 请求/命令模型放置在 `RequirementTrackingSystem.Primary/Contracts/` 下
- 响应模型与命令成对出现，使用 `Command + Response` 模式
- 列表响应应包含分页信息

### 示例

```csharp
public class CreateRequirementCommand : IMapFrom<Requirement>
{
    public string Name { get; set; }
    public string RequirementNo { get; set; }
    public int ProjectId { get; set; }
    public int FollowerId { get; set; }
    public Priority Priority { get; set; }
    public DateTime? PlanStartDate { get; set; }
    public DateTime? PlanTestDate { get; set; }
    public DateTime? PlanLaunchDate { get; set; }
    public string? DocUrl { get; set; }
    public decimal? Price { get; set; }
    public string? Remark { get; set; }
}

public class CreateRequirementResponse
{
    public int Id { get; set; }
}
```

## 10. 数据库操作

- 使用 Entity Framework Core 进行数据持久化
- 在 Handler 中无需手动调用 `SaveChanges`，Mediator Pipeline 自动处理事务
- 使用 `ApplicationDbContext` 访问数据库

### DbSet 配置

在 `ApplicationDbContext` 中添加 DbSet：

```csharp
public DbSet<Requirement> Requirements { get; set; }
public DbSet<Project> Projects { get; set; }
public DbSet<User> Users { get; set; }
public DbSet<Robot> Robots { get; set; }
public DbSet<NotificationLog> NotificationLogs { get; set; }
```

## 11. 依赖注入

- 使用构造函数注入获取依赖
- 常用依赖：`ApplicationDbContext`、`IMediator`、`ICurrentUser`
- 在 Handler 构造函数中注入

## 12. 验证规则

| 字段 | 验证规则 |
|------|---------|
| 需求名称 | 必填，最多 100 字 |
| 需求号 | 必填，唯一，最多 50 字 |
| 计划交测时间 | 必须在计划开始时间之后 |
| 计划上线时间 | 必须在计划交测时间之后 |
| 需求文档链接 | 必须为 http:// 或 https:// 开头的有效 URL |
| 报价 | 精度最多 2 位小数，范围 ≥0，仅管理员可见 |
| 版本号 | 初始为 1，每次更新后自增 |