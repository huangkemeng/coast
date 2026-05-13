# 需求跟踪管理系统 - C# 后端代码计划

## 1. 项目概述

| 属性 | 说明 |
|------|------|
| **项目名称** | RequirementTrackingSystem |
| **技术栈** | .NET 8 + Entity Framework Core |
| **架构模式** | Clean Architecture + CQRS |
| **API风格** | RESTful API |
| **目标** | 构建需求跟踪管理的后端服务，支持状态流转、通知推送、权限管理 |

---

## 2. 项目架构

```
RequirementTrackingSystem/
├── src/
│   ├── RequirementTrackingSystem.API/           # Web API层（控制器、中间件）
│   ├── RequirementTrackingSystem.Application/  # 应用层（用例、业务逻辑）
│   ├── RequirementTrackingSystem.Domain/        # 领域层（实体、值对象、领域服务）
│   └── RequirementTrackingSystem.Infrastructure/# 基础设施层（EF Core、第三方服务）
├── tests/
│   └── RequirementTrackingSystem.Tests/         # 单元测试
└── RequirementTrackingSystem.sln
```

### 2.1 分层职责

| 层 | 职责 | 依赖关系 |
|---|---|---|
| **API** | HTTP请求处理、路由、认证授权 | 依赖Application |
| **Application** | 用例编排、业务流程控制 | 依赖Domain、Infrastructure |
| **Domain** | 业务规则、实体状态、领域事件 | 无外部依赖 |
| **Infrastructure** | 数据持久化、外部服务调用 | 依赖Domain |

---

## 3. 数据模型设计

### 3.1 实体关系图

```
┌─────────────┐     ┌─────────────┐     ┌─────────────┐
│    User     │     │   Project   │     │   Robot     │
├─────────────┤     ├─────────────┤     ├─────────────┤
│ Id          │     │ Id          │     │ Id          │
│ Username    │     │ Name        │     │ Name        │
│ RealName    │     │ Code        │     │ WebhookUrl  │
│ Role        │     │ ManagerId   │     │ GroupName   │
│ Phone       │     │ Description │     │ IsEnabled   │
│ Email       │     │ CreatedAt   │     │ CreatedAt   │
│ IsEnabled   │     └──────┬──────┘     └──────┬──────┘
└──────┬──────┘            │                   │
       │                   │                   │
       │              ┌────┴───────────────────┘
       │              │        1:N
       │              ↓
       │    ┌─────────────────┐         ┌───────────────┐
       │    │   Requirement   │─────────│ NotificationLog│
       │    ├─────────────────┤   1:N   ├───────────────┤
       │    │ Id              │         │ Id            │
       │    │ Name           │         │ RequirementId │
       │    │ RequirementNo  │         │ Type          │
       │    │ Status         │         │ RobotId       │
       │    │ Progress       │         │ Status        │
       │    │ FollowerId     │         │ SentAt        │
       │    │ PlanStartDate  │         │ ErrorMessage  │
       │    │ PlanTestDate   │         └───────────────┘
       │    │ PlanLaunchDate │         
       │    │ ActualTestDate │         
       │    │ ActualLaunchDate│        
       │    │ IsConfirmed    │         
       │    │ DocUrl         │         
       │    │ Price          │         
       │    │ ProjectId      │         
       │    │ RobotId        │         
       │    │ Priority       │         
       │    │ Remark         │         
       │    │ Version        │  ← 乐观锁
       │    │ CreatedAt      │         
       │    │ UpdatedAt      │         
       │    └─────────────────┘         
       │            │
       └────────────┘           
        
┌─────────────────┐
│   NotificationJob│ (定时任务)
├─────────────────┤
│ Id              │
│ RequirementId   │
│ Type            │
│ ScheduledAt     │
│ SentAt          │
│ Status          │
└─────────────────┘
```

### 3.2 核心实体定义

#### Requirement（需求）
```csharp
public class Requirement
{
    public int Id { get; set; }
    public string Name { get; set; }                    // 需求名称
    public string RequirementNo { get; set; }          // 需求号
    public RequirementStatus Status { get; set; }      // 状态
    public int Progress { get; set; }                  // 进度 0-100
    public int FollowerId { get; set; }               // 跟进人
    public User Follower { get; set; }
    
    public DateTime? PlanStartDate { get; set; }       // 计划开始时间
    public DateTime? PlanTestDate { get; set; }        // 计划交测时间
    public DateTime? PlanLaunchDate { get; set; }      // 计划上线时间
    public DateTime? ActualTestDate { get; set; }       // 实际交测时间
    public DateTime? ActualLaunchDate { get; set; }    // 实际上线时间
    
    public bool IsConfirmed { get; set; }              // 需求已确认
    public string? DocUrl { get; set; }                // 需求文档链接
    public decimal? Price { get; set; }               // 报价
    public int ProjectId { get; set; }                 // 所属项目
    public Project Project { get; set; }
    public int? RobotId { get; set; }                  // 通知机器人
    public Robot? Robot { get; set; }
    public Priority Priority { get; set; }             // 优先级
    public string? Remark { get; set; }                // 备注
    
    public int Version { get; set; }                   // 乐观锁版本号
    
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
```

#### RequirementStatus（枚举）
```csharp
public enum RequirementStatus
{
    PendingConfirm = 0,    // 待确认
    Confirmed = 1,         // 已确认
    PendingQuote = 2,      // 待报价
    Quoted = 3,            // 已报价
    PendingDev = 4,        // 待开发
    InDev = 5,             // 开发中
    InTest = 6,            // 测试中
    AcceptedPendingLaunch = 7,  // 已验收待上线
    Launched = 8           // 已上线
}
```

#### User（用户）
```csharp
public class User
{
    public int Id { get; set; }
    public string Username { get; set; }              // 用户名
    public string RealName { get; set; }               // 姓名
    public UserRole Role { get; set; }                 // 角色
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public bool IsEnabled { get; set; }
    public DateTime CreatedAt { get; set; }
}

public enum UserRole
{
    Admin = 0,           // 管理员
    Developer = 1,       // 开发人员
    Tester = 2           // 测试人员
}
```

---

## 4. API 设计

### 4.1 需求管理 API

| 方法 | 端点 | 描述 | 权限 |
|------|------|------|------|
| GET | /api/requirements | 获取需求列表（支持筛选、分页、排序） | 所有用户 |
| GET | /api/requirements/{id} | 获取需求详情 | 所有用户 |
| POST | /api/requirements | 创建需求 | 管理员 |
| PUT | /api/requirements/{id} | 更新需求 | 管理员/跟进人 |
| DELETE | /api/requirements/{id} | 删除需求 | 管理员 |
| PUT | /api/requirements/{id}/status | 更新需求状态 | 管理员 |
| GET | /api/requirements/export | 导出需求列表 | 管理员 |

**筛选参数**：
- `status`: 状态（多个用逗号分隔）
- `followerId`: 跟进人
- `projectId`: 项目
- `planStartDateFrom/To`: 计划开始时间范围
- `planTestDateFrom/To`: 计划交测时间范围
- `pageIndex`: 页码（默认1）
- `pageSize`: 每页条数（默认20，支持10/20/50）
- `sortBy`: 排序字段
- `sortOrder`: 排序方向（asc/desc）

### 4.2 项目管理 API

| 方法 | 端点 | 描述 | 权限 |
|------|------|------|------|
| GET | /api/projects | 获取项目列表 | 所有用户 |
| GET | /api/projects/{id} | 获取项目详情 | 所有用户 |
| POST | /api/projects | 创建项目 | 管理员 |
| PUT | /api/projects/{id} | 更新项目 | 管理员 |
| DELETE | /api/projects/{id} | 删除项目 | 管理员 |

### 4.3 机器人配置 API

| 方法 | 端点 | 描述 | 权限 |
|------|------|------|------|
| GET | /api/robots | 获取机器人列表 | 管理员 |
| GET | /api/robots/{id} | 获取机器人详情 | 管理员 |
| POST | /api/robots | 创建机器人 | 管理员 |
| PUT | /api/robots/{id} | 更新机器人 | 管理员 |
| DELETE | /api/robots/{id} | 删除机器人 | 管理员 |
| POST | /api/robots/{id}/test | 测试机器人连接 | 管理员 |

### 4.4 通知日志 API

| 方法 | 端点 | 描述 | 权限 |
|------|------|------|------|
| GET | /api/notifications | 获取通知日志列表 | 管理员 |
| GET | /api/notifications/{id} | 获取通知详情 | 管理员 |

### 4.5 用户管理 API

| 方法 | 端点 | 描述 | 权限 |
|------|------|------|------|
| GET | /api/users | 获取用户列表 | 管理员 |
| GET | /api/users/{id} | 获取用户详情 | 所有用户 |
| POST | /api/users | 创建用户 | 管理员 |
| PUT | /api/users/{id} | 更新用户 | 管理员 |
| DELETE | /api/users/{id} | 删除用户 | 管理员 |

---

## 5. 核心业务逻辑

### 5.1 状态流转服务

```csharp
public class RequirementStateMachine
{
    // 定义合法流转路径
    private static readonly Dictionary<RequirementStatus, RequirementStatus[]> 
        ValidTransitions = new()
    {
        { RequirementStatus.PendingConfirm, new[] { RequirementStatus.Confirmed } },
        { RequirementStatus.Confirmed, new[] { RequirementStatus.PendingQuote } },
        { RequirementStatus.PendingQuote, new[] { RequirementStatus.Quoted } },
        { RequirementStatus.Quoted, new[] { RequirementStatus.PendingDev } },
        { RequirementStatus.PendingDev, new[] { RequirementStatus.InDev } },
        { RequirementStatus.InDev, new[] { RequirementStatus.InTest } },
        { RequirementStatus.InTest, new[] { RequirementStatus.AcceptedPendingLaunch } },
        { RequirementStatus.AcceptedPendingLaunch, new[] { RequirementStatus.Launched } },
        { RequirementStatus.Launched, Array.Empty<RequirementStatus>() } // 终态
    };

    public bool CanTransition(RequirementStatus from, RequirementStatus to)
    {
        return ValidTransitions.TryGetValue(from, out var validTargets) 
               && validTargets.Contains(to);
    }

    public RequirementStatus? GetNextStatus(RequirementStatus current)
    {
        return ValidTransitions.TryGetValue(current, out var targets) 
               && targets.Length > 0 ? targets[0] : null;
    }
}
```

### 5.2 乐观锁实现

```csharp
public async Task<Result> UpdateRequirement(int id, UpdateRequirementDto dto, int expectedVersion)
{
    var requirement = await _context.Requirements.FindAsync(id);
    
    if (requirement == null)
        return Result.NotFound();
    
    if (requirement.Version != expectedVersion)
        return Result.Conflict("数据已被他人修改，请刷新页面获取最新数据后重新编辑");
    
    // 更新字段
    requirement.Name = dto.Name;
    requirement.Status = dto.Status;
    // ... 其他字段
    
    requirement.Version++;  // 版本号自增
    requirement.UpdatedAt = DateTime.UtcNow;
    
    await _context.SaveChangesAsync();
    
    return Result.Success(requirement);
}
```

### 5.3 状态自动逻辑

| 触发条件 | 自动操作 |
|----------|----------|
| 状态变更为"已确认"及以上 | `IsConfirmed = true` |
| 状态回退为"待确认" | `IsConfirmed = false` |
| 状态变更为"测试中" | 自动填充 `ActualTestDate` |
| 状态变更为"已上线" | 自动填充 `ActualLaunchDate` |

### 5.4 企业微信通知服务

```csharp
public class WeChatWorkNotifier : INotificationService
{
    private readonly HttpClient _httpClient;
    private readonly INotificationLogRepository _logRepo;

    public async Task<Result> SendStatusChangeNotification(Requirement requirement, 
        RequirementStatus oldStatus, RequirementStatus newStatus)
    {
        var robot = requirement.Robot;
        if (robot == null || !robot.IsEnabled)
            return Result.Success(); // 无机器人配置，跳过

        var message = BuildStatusChangeMessage(requirement, oldStatus, newStatus);
        
        var result = await SendToWeChatWork(robot.WebhookUrl, message);
        
        // 记录日志
        await _logRepo.AddAsync(new NotificationLog
        {
            RequirementId = requirement.Id,
            Type = NotificationType.StatusChange,
            RobotId = robot.Id,
            Status = result.IsSuccess ? NotificationStatus.Success : NotificationStatus.Failed,
            ErrorMessage = result.ErrorMessage,
            SentAt = DateTime.UtcNow
        });
        
        return result;
    }

    private string BuildStatusChangeMessage(Requirement req, 
        RequirementStatus oldStatus, RequirementStatus newStatus)
    {
        return $@"【需求状态变更】
需求名称：{req.Name}
需求号：{req.RequirementNo}
状态变更：{GetStatusName(oldStatus)} → {GetStatusName(newStatus)}
跟进人：{req.Follower?.RealName}
计划交测时间：{req.PlanTestDate:yyyy-MM-dd}
实际交测时间：{req.ActualTestDate?.ToString("yyyy-MM-dd") ?? "-"}
备注：{req.Remark ?? "-"}";
    }
}
```

### 5.5 通知重试策略

```csharp
public class NotificationRetryPolicy
{
    private static readonly int[] RetryIntervals = { 30, 120, 300 }; // 秒

    public async Task ProcessFailedNotifications()
    {
        var failedLogs = await _notificationLogRepo
            .GetFailedWithRetryCountLessThan(3);

        foreach (var log in failedLogs)
        {
            var delay = RetryIntervals[log.RetryCount];
            var timeSinceLastAttempt = DateTime.UtcNow - log.LastAttemptAt;
            
            if (timeSinceLastAttempt.TotalSeconds >= delay)
            {
                await RetryNotification(log);
            }
        }
    }
}
```

### 5.6 时间提醒服务

```csharp
public class ReminderService : BackgroundService
{
    protected async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var now = DateTime.UtcNow;
            var targetTime = GetNext9AM();
            
            if (now >= targetTime)
            {
                await SendDailyReminders();
                await ProcessFailedNotificationRetries();
            }
            
            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }

    private async Task SendDailyReminders()
    {
        var requirements = await _requirementRepo
            .GetPendingReminders(GetReminderThreshold());

        var groupedByPriority = requirements.GroupBy(r => r.Priority);

        foreach (var group in groupedByPriority)
        {
            var reminderDays = GetReminderDays(group.Key);
            
            foreach (var req in group)
            {
                if (ShouldRemind(req, reminderDays))
                {
                    await _notificationService.SendReminder(req);
                }
            }
        }
    }

    private int[] GetReminderDays(Priority priority) => priority switch
    {
        Priority.High => new[] { 3, 1, 0 },
        Priority.Medium => new[] { 2, 0 },
        Priority.Low => new[] { 1 },
        _ => Array.Empty<int>()
    };
}
```

---

## 6. 权限控制

### 6.1 角色权限矩阵

| 操作 | 管理员 | 开发人员 | 测试人员 |
|------|--------|----------|----------|
| 需求增删改查 | ✅ | 查看、更新分配给自己的 | 查看、更新分配给自己的 |
| 需求状态变更 | ✅ | ❌ | ❌ |
| 需求报价查看/编辑 | ✅ | ❌ | ❌ |
| 项目管理 | ✅ | ❌ | ❌ |
| 机器人配置 | ✅ | ❌ | ❌ |
| 用户管理 | ✅ | ❌ | ❌ |
| 通知日志查看 | ✅ | ❌ | ❌ |

### 6.2 权限实现

```csharp
public class RequirementAuthorizationHandler : 
    AuthorizationHandler<OperationRequirement, Requirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        OperationRequirement requirement,
        Requirement resource)
    {
        var user = _userContext.GetCurrentUser();

        if (user.Role == UserRole.Admin)
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        switch (requirement.Operation)
        {
            case Operation.Read:
                if (resource.FollowerId == user.Id)
                    context.Succeed(requirement);
                break;
            case Operation.Update:
                if (resource.FollowerId == user.Id)
                    context.Succeed(requirement);
                break;
            case Operation.Delete:
            case Operation.ChangeStatus:
            case Operation.ViewPrice:
                // 仅管理员可执行
                break;
        }

        return Task.CompletedTask;
    }
}
```

---

## 7. 数据校验规则

| 字段 | 校验规则 |
|------|----------|
| 需求名称 | 必填，最多100字 |
| 需求号 | 必填，唯一，最多50字 |
| 计划交测时间 | 必须在计划开始时间之后 |
| 计划上线时间 | 必须在计划交测时间之后 |
| 需求文档链接 | 必须为 http:// 或 https:// 开头的有效URL，不支持内网地址 |
| 报价 | 精度最多2位小数，范围 ≥0，仅管理员可见 |
| 版本号 | 初始为1，每次更新后自增 |

---

## 8. 文件结构规划

```
src/
├── RequirementTrackingSystem.API/
│   ├── Controllers/
│   │   ├── RequirementsController.cs
│   │   ├── ProjectsController.cs
│   │   ├── RobotsController.cs
│   │   ├── UsersController.cs
│   │   └── NotificationsController.cs
│   ├── Middleware/
│   │   └── ExceptionHandlingMiddleware.cs
│   ├── Filters/
│   │   └── ValidateModelAttribute.cs
│   ├── Program.cs
│   └── appsettings.json
│
├── RequirementTrackingSystem.Application/
│   ├── Common/
│   │   ├── Result.cs
│   │   ├── Pagination.cs
│   │   └── CurrentUser.cs
│   ├── Requirements/
│   │   ├── Queries/
│   │   │   ├── GetRequirementsQuery.cs
│   │   │   └── GetRequirementByIdQuery.cs
│   │   ├── Commands/
│   │   │   ├── CreateRequirementCommand.cs
│   │   │   ├── UpdateRequirementCommand.cs
│   │   │   └── ChangeRequirementStatusCommand.cs
│   │   └── DTOs/
│   │       ├── RequirementDto.cs
│   │       └── UpdateRequirementDto.cs
│   ├── Projects/
│   ├── Robots/
│   ├── Users/
│   └── Services/
│       ├── INotificationService.cs
│       ├── RequirementStateMachine.cs
│       └── IReminderService.cs
│
├── RequirementTrackingSystem.Domain/
│   ├── Entities/
│   │   ├── Requirement.cs
│   │   ├── Project.cs
│   │   ├── Robot.cs
│   │   ├── User.cs
│   │   └── NotificationLog.cs
│   ├── Enums/
│   │   ├── RequirementStatus.cs
│   │   ├── UserRole.cs
│   │   └── Priority.cs
│   ├── ValueObjects/
│   │   └── Url.cs
│   └── Exceptions/
│       └── DomainException.cs
│
└── RequirementTrackingSystem.Infrastructure/
    ├── Data/
    │   ├── AppDbContext.cs
    │   └── Configurations/
    │       ├── RequirementConfiguration.cs
    │       ├── ProjectConfiguration.cs
    │       ├── RobotConfiguration.cs
    │       └── UserConfiguration.cs
    ├── Repositories/
    │   ├── RequirementRepository.cs
    │   ├── ProjectRepository.cs
    │   ├── RobotRepository.cs
    │   └── NotificationLogRepository.cs
    ├── Services/
    │   ├── WeChatWorkNotifier.cs
    │   └── ReminderBackgroundService.cs
    └── Extensions/
        └── ServiceCollectionExtensions.cs

tests/
└── RequirementTrackingSystem.Tests/
    ├── Domain/
    │   └── RequirementStateMachineTests.cs
    ├── Application/
    │   └── Requirements/
    │       └── UpdateRequirementCommandTests.cs
    └── Infrastructure/
        └── RequirementRepositoryTests.cs
```

---

## 9. 实现优先级

### Phase 1 - MVP（Must-have）

1. **项目基础架构搭建**
   - 解决方案和项目结构
   - 依赖注入配置
   - 数据库上下文和迁移

2. **实体和仓储层**
   - Domain 实体定义
   - EF Core 配置
   - 基础仓储实现

3. **需求管理核心**
   - CRUD API 实现
   - 状态流转逻辑
   - 并发控制（乐观锁）

4. **项目管理**
   - 项目 CRUD

5. **用户管理**
   - 用户 CRUD
   - 角色权限基础

6. **企业微信通知**
   - 机器人配置
   - 状态变更通知发送

### Phase 2 - Should-have

7. **时间提醒通知**
   - 定时任务服务
   - 按优先级差异化提醒

8. **通知日志**
   - 通知记录查询
   - 重试机制

9. **高级功能**
   - 列表筛选排序
   - 数据导出 Excel

### Phase 3 - Could-have

10. **统计报表**
11. **批量操作**

---

## 10. 关键技术点

### 10.1 状态流转校验

前端仅显示合法后继状态，后端二次校验：
- 使用状态机模式验证流转合法性
- 拦截非法状态变更请求

### 10.2 乐观锁并发控制

- 使用 `version` 字段实现
- 更新时检查版本号匹配
- 返回冲突错误引导用户刷新

### 10.3 软删除 vs 硬删除

需求删除采用**硬删除**（PRD明确），但需先检查关联的跟进人/项目/机器人

### 10.4 后台任务

使用 `IHostedService` 实现定时任务：
- 每日9:00发送提醒
- 通知重试扫描

---

## 12. 删除级联逻辑

### 12.1 用户删除约束

```csharp
public class UserService
{
    public async Task<Result> DeleteUser(int userId)
    {
        var user = await _userRepo.GetByIdAsync(userId);
        
        var requirementCount = await _requirementRepo.CountByFollowerIdAsync(userId);
        
        if (requirementCount > 0)
        {
            return Result.BadRequest(
                $"该用户是 {requirementCount} 条需求的跟进人，无法删除。请先变更这些需求的跟进人后再删除用户");
        }
        
        // 软删除用户（设置 IsEnabled = false）
        user.IsEnabled = false;
        user.DeletedAt = DateTime.UtcNow;
        
        await _userRepo.UpdateAsync(user);
        
        return Result.Success();
    }
}
```

### 12.2 机器人删除级联

```csharp
public class RobotService
{
    public async Task<Result> DeleteRobot(int robotId)
    {
        var robot = await _robotRepo.GetByIdAsync(robotId);
        
        // 自动清除关联需求的机器人引用
        var requirements = await _requirementRepo.GetByRobotIdAsync(robotId);
        
        foreach (var req in requirements)
        {
            req.RobotId = null;
            
            // 记录通知日志
            await _notificationLogRepo.AddAsync(new NotificationLog
            {
                RequirementId = req.Id,
                Type = NotificationType.CascadedClear,
                Status = NotificationStatus.Success,
                Remark = "机器人已删除，关联清除",
                CreatedAt = DateTime.UtcNow
            });
        }
        
        await _requirementRepo.UpdateRangeAsync(requirements);
        
        // 硬删除机器人
        await _robotRepo.DeleteAsync(robotId);
        
        return Result.Success();
    }
}
```

### 12.3 项目删除约束

```csharp
public class ProjectService
{
    public async Task<Result> DeleteProject(int projectId)
    {
        var requirementCount = await _requirementRepo.CountByProjectIdAsync(projectId);
        
        if (requirementCount > 0)
        {
            return Result.BadRequest("该项目下存在需求，无法删除");
        }
        
        await _projectRepo.DeleteAsync(projectId);
        
        return Result.Success();
    }
}
```

---

## 13. 通知重试机制详细设计

### 13.1 NotificationLog 实体扩展

```csharp
public class NotificationLog
{
    public int Id { get; set; }
    public int RequirementId { get; set; }
    public NotificationType Type { get; set; }
    public int? RobotId { get; set; }
    public NotificationStatus Status { get; set; }
    public string? ErrorMessage { get; set; }
    public int RetryCount { get; set; }        // 重试次数
    public DateTime? LastAttemptAt { get; set; } // 最后尝试时间
    public DateTime SentAt { get; set; }
}

public enum NotificationType
{
    StatusChange = 0,
    Reminder = 1,
    CascadedClear = 2  // 机器人删除时的级联清除通知
}

public enum NotificationStatus
{
    Pending = 0,
    Success = 1,
    Failed = 2,
    Retrying = 3
}
```

### 13.2 重试服务实现

```csharp
public class NotificationRetryService : BackgroundService
{
    private readonly int[] _retryIntervals = { 30, 120, 300 }; // 秒
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await ProcessFailedNotifications();
            await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
        }
    }
    
    public async Task ProcessFailedNotifications()
    {
        var failedLogs = await _logRepo.GetPendingRetries();
        
        foreach (var log in failedLogs)
        {
            if (log.RetryCount >= 3)
            {
                log.Status = NotificationStatus.Failed;
                continue;
            }
            
            var delay = _retryIntervals[log.RetryCount];
            var elapsed = DateTime.UtcNow - log.LastAttemptAt;
            
            if (elapsed.TotalSeconds >= delay)
            {
                await RetryNotification(log);
            }
        }
    }
    
    private async Task RetryNotification(NotificationLog log)
    {
        log.RetryCount++;
        log.LastAttemptAt = DateTime.UtcNow;
        log.Status = NotificationStatus.Retrying;
        
        var result = await _notifier.SendAsync(log);
        
        if (result.IsSuccess)
        {
            log.Status = NotificationStatus.Success;
        }
        else
        {
            log.ErrorMessage = result.ErrorMessage;
            log.Status = log.RetryCount >= 3 
                ? NotificationStatus.Failed 
                : NotificationStatus.Pending;
        }
        
        await _logRepo.UpdateAsync(log);
    }
}
```

### 13.3 每日补偿扫描

```csharp
public class DailyCompensationJob : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var now = DateTime.UtcNow;
            var next9AM = GetNext9AM(now);
            var delay = next9AM - now;
            
            await Task.Delay(delay, stoppingToken);
            
            await RetryPendingNotifications();
        }
    }
    
    private async Task RetryPendingNotifications()
    {
        var yesterdayFailed = await _logRepo.GetYesterdayFailed();
        
        foreach (var log in yesterdayFailed)
        {
            var result = await _notifier.SendAsync(log);
            
            log.RetryCount++;
            log.LastAttemptAt = DateTime.UtcNow;
            log.Status = result.IsSuccess 
                ? NotificationStatus.Success 
                : NotificationStatus.Failed;
            log.ErrorMessage = result.ErrorMessage;
            
            await _logRepo.UpdateAsync(log);
        }
    }
}
```

---

## 14. 字段级权限控制

### 14.1 DTO 设计（分离公开字段）

```csharp
// 管理员可见的完整 DTO
public class RequirementAdminDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string RequirementNo { get; set; }
    public RequirementStatus Status { get; set; }
    public int Progress { get; set; }
    public UserDto Follower { get; set; }
    public DateTime? PlanStartDate { get; set; }
    public DateTime? PlanTestDate { get; set; }
    public DateTime? PlanLaunchDate { get; set; }
    public DateTime? ActualTestDate { get; set; }
    public DateTime? ActualLaunchDate { get; set; }
    public bool IsConfirmed { get; set; }
    public string? DocUrl { get; set; }
    public decimal? Price { get; set; }  // 仅管理员可见
    public ProjectDto Project { get; set; }
    public RobotDto? Robot { get; set; }
    public Priority Priority { get; set; }
    public string? Remark { get; set; }
    public int Version { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

// 普通用户可见的 DTO（不包含 Price）
public class RequirementUserDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string RequirementNo { get; set; }
    public RequirementStatus Status { get; set; }
    public int Progress { get; set; }
    public UserDto Follower { get; set; }
    public DateTime? PlanStartDate { get; set; }
    public DateTime? PlanTestDate { get; set; }
    public DateTime? PlanLaunchDate { get; set; }
    public DateTime? ActualTestDate { get; set; }
    public DateTime? ActualLaunchDate { get; set; }
    public bool IsConfirmed { get; set; }
    public string? DocUrl { get; set; }
    public decimal? Price { get; set; }  // 始终为 null
    public ProjectDto Project { get; set; }
    public RobotDto? Robot { get; set; }
    public Priority Priority { get; set; }
    public string? Remark { get; set; }
    public int Version { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
```

### 14.2 控制器返回策略

```csharp
[HttpGet("{id}")]
public async Task<ActionResult<RequirementDto>> GetById(int id)
{
    var currentUser = _authService.GetCurrentUser();
    
    var requirement = await _requirementService.GetByIdAsync(id);
    
    if (currentUser.Role == UserRole.Admin)
    {
        return Ok(_mapper.Map<RequirementAdminDto>(requirement));
    }
    else
    {
        var dto = _mapper.Map<RequirementUserDto>(requirement);
        dto.Price = null;  // 确保不返回报价
        return Ok(dto);
    }
}
```

### 14.3 编辑权限控制

```csharp
[HttpPut("{id}")]
public async Task<ActionResult> Update(int id, [FromBody] UpdateRequirementDto dto)
{
    var currentUser = _authService.GetCurrentUser();
    var requirement = await _requirementService.GetByIdAsync(id);
    
    if (currentUser.Role != UserRole.Admin)
    {
        if (requirement.FollowerId != currentUser.Id)
            return Forbid();
        
        // 非管理员不可编辑 Price
        if (dto.Price != requirement.Price)
            return BadRequest("您没有权限修改报价字段");
    }
    
    await _requirementService.UpdateAsync(id, dto, currentUser);
    
    return Ok();
}
```

---

## 15. 定时任务设计

### 15.1 任务概览

| 任务 | 执行时间 | 说明 |
|------|----------|------|
| 时间提醒扫描 | 每天 09:00 | 检查到期需求，发送提醒通知 |
| 失败通知重试 | 每 10 秒 | 扫描待重试通知，执行重试逻辑 |
| 每日补偿扫描 | 每天 09:00 | 重试过去 24 小时内失败的的通知 |

### 15.2 提醒规则实现

```csharp
public class ReminderService : BackgroundService
{
    private readonly Dictionary<Priority, int[]> _reminderDays = new()
    {
        { Priority.High, new[] { 3, 1, 0 } },      // 高：3天、1天、当天
        { Priority.Medium, new[] { 2, 0 } },      // 中：2天、当天
        { Priority.Low, new[] { 1 } }             // 低：1天
    };
    
    private async Task SendDailyReminders()
    {
        var today = DateTime.UtcNow.Date;
        
        var requirements = await _requirementRepo.GetPendingReminders();
        
        foreach (var req in requirements)
        {
            var reminderDays = _reminderDays[req.Priority];
            
            // 检查交测提醒
            if (req.PlanTestDate.HasValue)
            {
                var daysUntil = (req.PlanTestDate.Value.Date - today).Days;
                if (reminderDays.Contains(daysUntil) && !await _logRepo.HasSentToday(req.Id, "Test", daysUntil))
                {
                    await _notifier.SendReminder(req, "交测", daysUntil);
                }
            }
            
            // 检查上线提醒
            if (req.PlanLaunchDate.HasValue)
            {
                var daysUntil = (req.PlanLaunchDate.Value.Date - today).Days;
                if (reminderDays.Contains(daysUntil) && !await _logRepo.HasSentToday(req.Id, "Launch", daysUntil))
                {
                    await _notifier.SendReminder(req, "上线", daysUntil);
                }
            }
        }
    }
}
```

---

## 16. 数据导出功能

### 16.1 导出 API

```csharp
[HttpGet("export")]
[Authorize(Roles = "Admin")]
public async Task<IActionResult> Export(
    [FromQuery] string? status,
    [FromQuery] int? followerId,
    [FromQuery] int? projectId,
    [FromQuery] DateTime? planStartDateFrom,
    [FromQuery] DateTime? planStartDateTo)
{
    var query = new GetRequirementsQuery
    {
        Status = status,
        FollowerId = followerId,
        ProjectId = projectId,
        PlanStartDateFrom = planStartDateFrom,
        PlanStartDateTo = planStartDateTo
    };
    
    var requirements = await _mediator.Send(query);
    
    var bytes = _excelExporter.Export(requirements, currentUser.Role);
    
    var fileName = $"需求列表_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
    
    return File(bytes, 
        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
        fileName);
}
```

### 16.2 Excel 导出策略

```csharp
public class ExcelExporter
{
    public byte[] Export(IEnumerable<RequirementDto> requirements, UserRole role)
    {
        var dataTable = new DataTable("需求列表");
        
        // 基础列（所有角色可见）
        dataTable.Columns.Add("需求名称");
        dataTable.Columns.Add("需求号");
        dataTable.Columns.Add("当前状态");
        dataTable.Columns.Add("进度");
        dataTable.Columns.Add("跟进人");
        dataTable.Columns.Add("计划开始时间");
        dataTable.Columns.Add("计划交测时间");
        dataTable.Columns.Add("计划上线时间");
        dataTable.Columns.Add("实际交测时间");
        dataTable.Columns.Add("实际上线时间");
        dataTable.Columns.Add("需求已确认");
        dataTable.Columns.Add("需求文档链接");
        dataTable.Columns.Add("所属系统");
        dataTable.Columns.Add("优先级");
        dataTable.Columns.Add("备注");
        dataTable.Columns.Add("创建时间");
        
        // 报价列（仅管理员可见）
        if (role == UserRole.Admin)
        {
            dataTable.Columns.Add("报价");
        }
        
        // 填充数据
        foreach (var req in requirements)
        {
            var row = dataTable.NewRow();
            row["需求名称"] = req.Name;
            row["需求号"] = req.RequirementNo;
            row["当前状态"] = GetStatusName(req.Status);
            row["进度"] = $"{req.Progress}%";
            row["跟进人"] = req.Follower?.RealName ?? "";
            row["计划开始时间"] = req.PlanStartDate?.ToString("yyyy-MM-dd") ?? "";
            row["计划交测时间"] = req.PlanTestDate?.ToString("yyyy-MM-dd") ?? "";
            row["计划上线时间"] = req.PlanLaunchDate?.ToString("yyyy-MM-dd") ?? "";
            row["实际交测时间"] = req.ActualTestDate?.ToString("yyyy-MM-dd") ?? "";
            row["实际上线时间"] = req.ActualLaunchDate?.ToString("yyyy-MM-dd") ?? "";
            row["需求已确认"] = req.IsConfirmed ? "是" : "否";
            row["需求文档链接"] = req.DocUrl ?? "";
            row["所属系统"] = req.Project?.Name ?? "";
            row["优先级"] = GetPriorityName(req.Priority);
            row["备注"] = req.Remark ?? "";
            row["创建时间"] = req.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss");
            
            if (role == UserRole.Admin)
            {
                row["报价"] = req.Price?.ToString("F2") ?? "";
            }
            
            dataTable.Rows.Add(row);
        }
        
        return _excelBuilder.Build(dataTable);
    }
}
```

---

## 17. 单元测试设计

### 17.1 测试项目结构

```
tests/
└── RequirementTrackingSystem.Tests/
    ├── Domain/
    │   ├── Entities/
    │   │   └── RequirementTests.cs
    │   └── Services/
    │       └── RequirementStateMachineTests.cs
    ├── Application/
    │   ├── Requirements/
    │   │   ├── Commands/
    │   │   │   ├── CreateRequirementCommandTests.cs
    │   │   │   ├── UpdateRequirementCommandTests.cs
    │   │   │   └── ChangeRequirementStatusCommandTests.cs
    │   │   └── Queries/
    │   │       └── GetRequirementsQueryTests.cs
    │   ├── Projects/
    │   │   └── ProjectServiceTests.cs
    │   ├── Users/
    │   │   └── UserServiceTests.cs
    │   └── Robots/
    │       └── RobotServiceTests.cs
    └── Infrastructure/
        └── Repositories/
            └── RequirementRepositoryTests.cs
```

### 17.2 状态机测试用例

```csharp
public class RequirementStateMachineTests
{
    [Theory]
    [InlineData(RequirementStatus.PendingConfirm, RequirementStatus.Confirmed, true)]
    [InlineData(RequirementStatus.Confirmed, RequirementStatus.PendingQuote, true)]
    [InlineData(RequirementStatus.Launched, RequirementStatus.Launched, false)] // 终态不可变
    [InlineData(RequirementStatus.PendingConfirm, RequirementStatus.InDev, false)] // 跳过中间状态
    [InlineData(RequirementStatus.InDev, RequirementStatus.PendingDev, false)] // 逆向流转
    public void CanTransition_ReturnsExpectedResult(
        RequirementStatus from, 
        RequirementStatus to, 
        bool expected)
    {
        var stateMachine = new RequirementStateMachine();
        
        var result = stateMachine.CanTransition(from, to);
        
        Assert.Equal(expected, result);
    }
    
    [Fact]
    public void GetNextStatus_WhenLaunched_ReturnsNull()
    {
        var stateMachine = new RequirementStateMachine();
        
        var result = stateMachine.GetNextStatus(RequirementStatus.Launched);
        
        Assert.Null(result);
    }
}
```

### 17.3 乐观锁测试用例

```csharp
public class UpdateRequirementCommandTests
{
    [Fact]
    public async Task Execute_WhenVersionMismatch_ReturnsConflict()
    {
        var repository = new Mock<IRequirementRepository>();
        var requirement = new Requirement { Id = 1, Version = 5 };
        repository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(requirement);
        
        var command = new UpdateRequirementCommand { Id = 1, Version = 3 }; // 旧版本
        
        var handler = new UpdateRequirementCommandHandler(repository.Object);
        var result = await handler.Handle(command);
        
        Assert.True(result.IsConflict);
        Assert.Contains("数据已被他人修改", result.Message);
    }
    
    [Fact]
    public async Task Execute_WhenVersionMatch_UpdatesSuccessfully()
    {
        var repository = new Mock<IRequirementRepository>();
        var requirement = new Requirement { Id = 1, Version = 3 };
        repository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(requirement);
        
        var command = new UpdateRequirementCommand { Id = 1, Version = 3 };
        
        var handler = new UpdateRequirementCommandHandler(repository.Object);
        var result = await handler.Handle(command);
        
        Assert.True(result.IsSuccess);
        Assert.Equal(4, requirement.Version); // 版本号自增
    }
}
```

### 17.4 删除约束测试用例

```csharp
public class UserServiceTests
{
    [Fact]
    public async Task DeleteUser_WhenHasAssociatedRequirements_ReturnsError()
    {
        var userRepo = new Mock<IUserRepository>();
        var reqRepo = new Mock<IRequirementRepository>();
        
        var userId = 1;
        reqRepo.Setup(r => r.CountByFollowerIdAsync(userId)).ReturnsAsync(5);
        
        var service = new UserService(userRepo.Object, reqRepo.Object);
        var result = await service.DeleteUser(userId);
        
        Assert.False(result.IsSuccess);
        Assert.Contains("5 条需求", result.Message);
    }
}

public class ProjectServiceTests
{
    [Fact]
    public async Task DeleteProject_WhenHasAssociatedRequirements_ReturnsError()
    {
        var projectRepo = new Mock<IProjectRepository>();
        var reqRepo = new Mock<IRequirementRepository>();
        
        var projectId = 1;
        reqRepo.Setup(r => r.CountByProjectIdAsync(projectId)).ReturnsAsync(3);
        
        var service = new ProjectService(projectRepo.Object, reqRepo.Object);
        var result = await service.DeleteProject(projectId);
        
        Assert.False(result.IsSuccess);
        Assert.Contains("存在需求", result.Message);
    }
}
```

---

## 18. API 详细规格

### 18.1 请求/响应模型

#### 创建需求请求
```json
POST /api/requirements
{
    "name": "用户登录功能",
    "requirementNo": "REQ-2025-001",
    "status": 0,
    "progress": 0,
    "followerId": 1,
    "planStartDate": "2025-06-01",
    "planTestDate": "2025-06-15",
    "planLaunchDate": "2025-06-20",
    "projectId": 1,
    "priority": 1,
    "docUrl": "https://docs.example.com/req-001",
    "price": 5000.00,
    "robotId": 1,
    "remark": "优先级较高"
}
```

#### 创建需求响应
```json
{
    "code": 200,
    "message": "success",
    "data": {
        "id": 1,
        "name": "用户登录功能",
        "requirementNo": "REQ-2025-001",
        "status": 0,
        "statusName": "待确认",
        "progress": 0,
        "follower": {
            "id": 1,
            "username": "zhangsan",
            "realName": "张三"
        },
        "planStartDate": "2025-06-01",
        "planTestDate": "2025-06-15",
        "planLaunchDate": "2025-06-20",
        "actualTestDate": null,
        "actualLaunchDate": null,
        "isConfirmed": false,
        "docUrl": "https://docs.example.com/req-001",
        "price": 5000.00,
        "project": {
            "id": 1,
            "name": "项目A"
        },
        "robot": {
            "id": 1,
            "name": "研发群机器人"
        },
        "priority": 1,
        "priorityName": "高",
        "remark": "优先级较高",
        "version": 1,
        "createdAt": "2025-06-01T10:00:00Z",
        "updatedAt": "2025-06-01T10:00:00Z"
    }
}
```

#### 更新需求请求（带版本号）
```json
PUT /api/requirements/1
{
    "name": "用户登录功能（优化）",
    "requirementNo": "REQ-2025-001",
    "status": 0,
    "progress": 10,
    "followerId": 1,
    "planStartDate": "2025-06-01",
    "planTestDate": "2025-06-15",
    "planLaunchDate": "2025-06-20",
    "projectId": 1,
    "priority": 1,
    "docUrl": "https://docs.example.com/req-001",
    "price": 5000.00,
    "robotId": 1,
    "remark": "优先级较高",
    "version": 1
}
```

#### 冲突响应
```json
{
    "code": 409,
    "message": "数据已被他人修改，请刷新页面获取最新数据后重新编辑",
    "data": null
}
```

#### 分页响应
```json
{
    "code": 200,
    "message": "success",
    "data": {
        "items": [...],
        "totalCount": 100,
        "pageIndex": 1,
        "pageSize": 20,
        "totalPages": 5
    }
}
```

### 18.2 错误码定义

| 错误码 | 说明 |
|--------|------|
| 200 | 成功 |
| 400 | 请求参数错误 |
| 401 | 未登录 |
| 403 | 无权限 |
| 404 | 资源不存在 |
| 409 | 并发冲突（乐观锁） |
| 500 | 服务器内部错误 |

---

## 19. 数据库设计

### 19.1 表结构

```sql
-- 用户表
CREATE TABLE Users (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Username NVARCHAR(50) NOT NULL UNIQUE,
    RealName NVARCHAR(100) NOT NULL,
    PasswordHash NVARCHAR(256) NOT NULL,
    Role INT NOT NULL DEFAULT 0,
    Phone NVARCHAR(20),
    Email NVARCHAR(100),
    IsEnabled BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    DeletedAt DATETIME2
);

-- 项目表
CREATE TABLE Projects (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL,
    Code NVARCHAR(50),
    ManagerId INT,
    Description NVARCHAR(500),
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    FOREIGN KEY (ManagerId) REFERENCES Users(Id)
);

-- 机器人表
CREATE TABLE Robots (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL,
    WebhookUrl NVARCHAR(500) NOT NULL,
    GroupName NVARCHAR(100),
    IsEnabled BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);

-- 需求表
CREATE TABLE Requirements (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL,
    RequirementNo NVARCHAR(50) NOT NULL UNIQUE,
    Status INT NOT NULL DEFAULT 0,
    Progress INT NOT NULL DEFAULT 0,
    FollowerId INT NOT NULL,
    PlanStartDate DATETIME2,
    PlanTestDate DATETIME2,
    PlanLaunchDate DATETIME2,
    ActualTestDate DATETIME2,
    ActualLaunchDate DATETIME2,
    IsConfirmed BIT NOT NULL DEFAULT 0,
    DocUrl NVARCHAR(500),
    Price DECIMAL(18,2),
    ProjectId INT NOT NULL,
    RobotId INT,
    Priority INT NOT NULL DEFAULT 1,
    Remark NVARCHAR(500),
    Version INT NOT NULL DEFAULT 1,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    FOREIGN KEY (FollowerId) REFERENCES Users(Id),
    FOREIGN KEY (ProjectId) REFERENCES Projects(Id),
    FOREIGN KEY (RobotId) REFERENCES Robots(Id)
);

-- 通知日志表
CREATE TABLE NotificationLogs (
    Id INT PRIMARY KEY IDENTITY(1,1),
    RequirementId INT NOT NULL,
    Type INT NOT NULL,
    RobotId INT,
    Status INT NOT NULL,
    ErrorMessage NVARCHAR(500),
    RetryCount INT NOT NULL DEFAULT 0,
    LastAttemptAt DATETIME2,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    FOREIGN KEY (RequirementId) REFERENCES Requirements(Id),
    FOREIGN KEY (RobotId) REFERENCES Robots(Id)
);
```

### 19.2 索引设计

```sql
-- 需求表索引
CREATE INDEX IX_Requirements_Status ON Requirements(Status);
CREATE INDEX IX_Requirements_FollowerId ON Requirements(FollowerId);
CREATE INDEX IX_Requirements_ProjectId ON Requirements(ProjectId);
CREATE INDEX IX_Requirements_PlanTestDate ON Requirements(PlanTestDate);
CREATE INDEX IX_Requirements_PlanLaunchDate ON Requirements(PlanLaunchDate);
CREATE INDEX IX_Requirements_CreatedAt ON Requirements(CreatedAt DESC);

-- 通知日志表索引
CREATE INDEX IX_NotificationLogs_Status_RetryCount 
    ON NotificationLogs(Status, RetryCount) 
    WHERE Status = 2; -- 仅失败记录
CREATE INDEX IX_NotificationLogs_CreatedAt ON NotificationLogs(CreatedAt DESC);
```

---

## 20. 配置说明

### appsettings.json 配置项

```json
{
  "WeChatWork": {
    "WebhookBaseUrl": "https://qyapi.weixin.qq.com/cgi-bin/webhook/send",
    "RequestTimeout": 30
  },
  "Notification": {
    "RetryIntervals": [30, 120, 300],
    "MaxRetryCount": 3,
    "ReminderTime": "09:00",
    "RetryScanIntervalSeconds": 10
  },
  "Pagination": {
    "DefaultPageSize": 20,
    "MaxPageSize": 100
  },
  "Validation": {
    "MaxNameLength": 100,
    "MaxRequirementNoLength": 50,
    "MaxRemarkLength": 500,
    "MaxDocUrlLength": 500
  }
}
```

---

## 21. 后续步骤

1. 确认数据库选型（SQL Server / PostgreSQL / MySQL）
2. 确定认证方案（JWT / Cookie）
3. 明确部署环境要求
4. 补充API接口详细文档

如需进一步细化某个模块的实现细节，请告知。

---

## 9. 通知管理模块详细设计

### 9.1 通知类型定义

| 通知类型 | 触发场景 | 发送时机 | 测试用例 |
|----------|----------|----------|----------|
| **StatusChange** | 需求状态变更 | 状态保存后立即触发 | TC-NOT-001~TC-NOT-005 |
| **DailyReminder** | 定时提醒 | 每日9:00 AM | TC-NOT-006~TC-NOT-008 |
| **DelayWarning** | 计划时间临近 | 每日9:00 AM扫描 | TC-NOT-009~TC-NOT-011 |
| **RetryNotification** | 通知发送失败 | 补偿扫描触发 | TC-NOT-012~TC-NOT-015 |

### 9.2 通知状态定义

```csharp
public enum NotificationStatus
{
    Pending = 0,        // 待发送
    Success = 1,       // 发送成功
    Failed = 2,        // 发送失败
    Retrying = 3       // 重试中
}

public enum NotificationType
{
    StatusChange = 1,  // 状态变更通知
    DailyReminder = 2, // 每日提醒
    DelayWarning = 3   // 延迟预警
}
```

### 9.3 通知日志实体

```csharp
public class NotificationLog
{
    public int Id { get; set; }
    public int RequirementId { get; set; }           // 关联需求
    public Requirement Requirement { get; set; }
    
    public NotificationType Type { get; set; }      // 通知类型
    public int? RobotId { get; set; }               // 机器人ID
    public Robot? Robot { get; set; }
    
    public NotificationStatus Status { get; set; }  // 发送状态
    public int RetryCount { get; set; }             // 重试次数
    public int MaxRetryCount { get; set; } = 3;    // 最大重试次数
    
    public DateTime? ScheduledAt { get; set; }      // 计划发送时间
    public DateTime? SentAt { get; set; }           // 实际发送时间
    public DateTime? LastAttemptAt { get; set; }   // 最后尝试时间
    
    public string? RequestPayload { get; set; }     // 请求payload
    public string? ResponseContent { get; set; }    // 响应内容
    public string? ErrorMessage { get; set; }       // 错误信息
    
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
```

### 9.4 通知发送服务接口

```csharp
public interface INotificationService
{
    /// <summary>
    /// 发送状态变更通知
    /// </summary>
    Task<Result> SendStatusChangeNotificationAsync(
        int requirementId, 
        RequirementStatus oldStatus, 
        RequirementStatus newStatus);

    /// <summary>
    /// 发送每日提醒
    /// </summary>
    Task<Result> SendDailyReminderAsync(int requirementId);

    /// <summary>
    /// 发送延迟预警
    /// </summary>
    Task<Result> SendDelayWarningAsync(int requirementId, int delayDays);

    /// <summary>
    /// 批量发送通知（定时任务调用）
    /// </summary>
    Task<Result> BatchSendNotificationsAsync(IEnumerable<int> requirementIds, NotificationType type);

    /// <summary>
    /// 重试失败的通知
    /// </summary>
    Task<Result> RetryFailedNotificationAsync(int notificationLogId);

    /// <summary>
    /// 处理补偿扫描（定时任务调用）
    /// </summary>
    Task ProcessCompensationScanAsync();
}
```

### 9.5 通知消息格式

#### 状态变更通知消息
```json
{
    "msgtype": "markdown",
    "markdown": {
        "content": "【需求状态变更】\n>需求名称：**需求A**\n>需求号：REQ-2025-001\n>状态变更：待确认 → 已确认\n>跟进人：张三\n>计划交测时间：2025-06-15\n>备注：优先级高，请尽快处理"
    }
}
```

#### 每日提醒消息
```json
{
    "msgtype": "text",
    "text": {
        "content": "【每日需求提醒】您有3个需求需要跟进：\n1. REQ-2025-001 - 需求A - 待开发\n2. REQ-2025-002 - 需求B - 开发中(65%)\n3. REQ-2025-003 - 需求C - 测试中"
    }
}
```

#### 延迟预警消息
```json
{
    "msgtype": "markdown",
    "markdown": {
        "content": "⚠️【需求延期预警】\n>需求名称：**需求B**\n>需求号：REQ-2025-002\n>当前状态：开发中\n>计划交测时间：2025-06-10\n>已延期：**3天**\n>请尽快处理！"
    }
}
```

### 9.6 通知重试策略

| 重试次数 | 延迟时间 | 说明 |
|----------|----------|------|
| 第1次重试 | 30秒 | 首次失败后30秒 |
| 第2次重试 | 2分钟 | 第二次失败后2分钟 |
| 第3次重试 | 5分钟 | 第三次失败后5分钟 |

```csharp
public class NotificationRetryStrategy
{
    private static readonly int[] RetryIntervals = { 30, 120, 300 }; // 秒
    private const int MaxRetryCount = 3;

    public int GetNextRetryDelay(int currentRetryCount)
    {
        if (currentRetryCount >= MaxRetryCount)
            return -1; // 达到最大重试次数，不再重试

        return RetryIntervals[currentRetryCount];
    }

    public bool ShouldRetry(int currentRetryCount)
    {
        return currentRetryCount < MaxRetryCount;
    }

    public DateTime GetNextRetryTime(int currentRetryCount)
    {
        var delay = GetNextRetryDelay(currentRetryCount);
        return delay > 0 ? DateTime.UtcNow.AddSeconds(delay) : DateTime.MaxValue;
    }
}
```

### 9.7 补偿扫描服务

```csharp
public class NotificationCompensationService : BackgroundService
{
    private readonly INotificationService _notificationService;
    private readonly ILogger<NotificationCompensationService> _logger;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                // 每分钟执行一次补偿扫描
                await _notificationService.ProcessCompensationScanAsync();
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "补偿扫描执行异常");
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
        }
    }
}
```

### 9.8 通知API端点

| 方法 | 端点 | 描述 | 权限 | 测试用例 |
|------|------|------|------|----------|
| GET | /api/notifications | 获取通知日志列表（支持筛选、分页） | 管理员 | TC-NOT-001 |
| GET | /api/notifications/{id} | 获取通知详情 | 管理员 | TC-NOT-002 |
| POST | /api/notifications/{id}/retry | 手动重试失败的通知 | 管理员 | TC-NOT-012 |
| GET | /api/notifications/stats | 获取通知统计（成功率、平均延迟） | 管理员 | TC-NOT-019 |

**筛选参数**：
- `requirementId`: 需求ID
- `type`: 通知类型（StatusChange/DailyReminder/DelayWarning）
- `status`: 发送状态（Pending/Success/Failed/Retrying）
- `startTime`: 开始时间
- `endTime`: 结束时间
- `pageIndex`: 页码
- `pageSize`: 每页条数

---

## 10. 定时任务设计

### 10.1 定时任务列表

| 任务名称 | 执行时间 | 说明 | 测试用例 |
|----------|----------|------|----------|
| **每日提醒扫描** | 每天9:00 AM | 扫描需要提醒的需求 | TC-NOT-006 |
| **通知重试处理** | 每分钟 | 处理失败的通知重试 | TC-NOT-012 |
| **补偿扫描** | 每分钟 | 扫描并处理超时未发送的通知 | TC-NOT-015 |
| **统计报表生成** | 每天凌晨 | 生成日报/周报/月报数据 | - |

### 10.2 每日提醒扫描规则

```csharp
public class DailyReminderScanner
{
    public async Task<List<Requirement>> ScanPendingReminders()
    {
        var reminderThreshold = DateTime.UtcNow.AddDays(7); // 未来7天
        
        var requirements = await _requirementRepo.GetActiveRequirements();
        
        return requirements.Where(r => 
            (r.Status == RequirementStatus.InDev || 
             r.Status == RequirementStatus.InTest) &&
            r.PlanTestDate <= reminderThreshold &&
            r.PlanTestDate >= DateTime.UtcNow
        ).ToList();
    }

    public async Task<Dictionary<Priority, List<ReminderInfo>>> GroupByPriority(
        List<Requirement> requirements)
    {
        var result = new Dictionary<Priority, List<ReminderInfo>>();
        
        foreach (var priority in Enum.GetValues<Priority>())
        {
            var group = requirements
                .Where(r => r.Priority == priority)
                .Select(r => new ReminderInfo
                {
                    RequirementId = r.Id,
                    RequirementName = r.Name,
                    RequirementNo = r.RequirementNo,
                    FollowerName = r.Follower?.RealName,
                    Status = r.Status,
                    PlanDate = r.PlanTestDate
                })
                .ToList();
            
            if (group.Any())
                result[priority] = group;
        }
        
        return result;
    }
}
```

### 10.3 延迟预警规则

```csharp
public class DelayWarningScanner
{
    private static readonly Dictionary<Priority, int[]> ReminderDays = new()
    {
        { Priority.High, new[] { 3, 1, 0 } },    // 高优先级：提前3天、1天、当天
        { Priority.Medium, new[] { 2, 0 } },    // 中优先级：提前2天、当天
        { Priority.Low, new[] { 1 } }            // 低优先级：提前1天
    };

    public List<DelayWarningInfo> ScanDelayedRequirements()
    {
        var today = DateTime.UtcNow.Date;
        
        return _requirementRepo.GetAll()
            .Where(r => r.Status == RequirementStatus.InDev || 
                       r.Status == RequirementStatus.InTest)
            .Where(r => r.PlanTestDate.HasValue)
            .Select(r => new
            {
                Requirement = r,
                DaysDiff = (today - r.PlanTestDate.Value.Date).Days
            })
            .Where(x => x.DaysDiff >= 0 || 
                       ReminderDays[x.Requirement.Priority].Contains(-x.DaysDiff))
            .Select(x => new DelayWarningInfo
            {
                Requirement = x.Requirement,
                DelayDays = x.DaysDiff,
                WarningLevel = GetWarningLevel(x.DaysDiff)
            })
            .ToList();
    }

    private WarningLevel GetWarningLevel(int delayDays) => delayDays switch
    {
        > 7 => WarningLevel.Critical,   // 严重延期
        > 3 => WarningLevel.High,        // 高度延期
        > 0 => WarningLevel.Medium,      // 中度延期
        _ => WarningLevel.Low            // 临近截止
    };
}
```

---

## 11. 非功能需求设计

### 11.1 性能要求

| 指标 | 要求 | 实现方案 | 测试用例 |
|------|------|----------|----------|
| **列表查询响应时间** | < 500ms（1000条数据以内） | 数据库索引 + 分页 + 异步查询 | - |
| **单条记录查询** | < 100ms | 主键索引 | - |
| **创建/更新操作** | < 300ms | 乐观锁 + 事务优化 | - |
| **并发处理能力** | 支持100并发请求 | 连接池 + 异步IO | TC-REQ-050~TC-REQ-053 |
| **通知发送延迟** | 状态变更后5分钟内送达 | 异步队列 + 重试机制 | TC-FLOW-020 |

### 11.2 性能优化实现

```csharp
// 11.2.1 数据库索引策略
public class RequirementIndexConfiguration
{
    public static void Configure(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Requirement>(entity =>
        {
            // 复合索引：常用筛选组合
            entity.HasIndex(r => new { r.Status, r.ProjectId, r.FollowerId });
            
            // 计划时间索引
            entity.HasIndex(r => r.PlanStartDate);
            entity.HasIndex(r => r.PlanTestDate);
            entity.HasIndex(r => r.PlanLaunchDate);
            
            // 唯一索引：需求号
            entity.HasIndex(r => r.RequirementNo).IsUnique();
            
            // 通知日志索引
            entity.HasIndex(r => new { r.Status, r.ScheduledAt });
            entity.HasIndex(r => new { r.RetryCount, r.LastAttemptAt });
        });
    }
}

// 11.2.2 异步查询优化
public class RequirementQueryOptimizer
{
    public async Task<PagedResult<RequirementDto>> GetRequirementsPagedAsync(
        GetRequirementsQuery query)
    {
        var queryable = _context.Requirements
            .AsNoTracking()  // 只读查询，禁用跟踪
            .AsSplitQuery(); // 拆分查询，避免笛卡尔积

        // 应用筛选（这些字段已建索引）
        queryable = ApplyFilters(queryable, query);

        // 获取总数（使用估算，避免全表扫描）
        var totalCount = await queryable.CountAsync();
        
        // 应用排序
        queryable = ApplySorting(queryable, query);

        // 应用分页
        var items = await queryable
            .Skip((query.PageIndex - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(r => new RequirementDto
            {
                Id = r.Id,
                Name = r.Name,
                Status = r.Status,
                // ... 其他字段，投影查询避免加载整个实体
            })
            .ToListAsync();

        return new PagedResult<RequirementDto>
        {
            Items = items,
            TotalCount = totalCount,
            PageIndex = query.PageIndex,
            PageSize = query.PageSize
        };
    }
}
```

### 11.3 安全要求

| 安全项 | 要求 | 实现方案 | 测试用例 |
|--------|------|----------|---------|
| **认证** | JWT Token认证 | Bearer Token | TC-REQ-013 |
| **授权** | 基于角色的权限控制 | RBAC | TC-REQ-029, TC-PROJ-007 |
| **URL验证** | 禁止内网地址 | 白名单校验 | TC-REQ-032~TC-REQ-033 |
| **输入校验** | 所有输入参数校验 | 数据注解 + FluentValidation | TC-REQ-016~TC-REQ-022 |
| **SQL注入** | 参数化查询 | EF Core参数化 | - |
| **XSS** | 输出编码 | JSON序列化 | - |
| **敏感数据** | 报价仅管理员可见 | 字段级别权限控制 | TC-REQ-041~TC-REQ-043 |

### 11.4 安全实现

```csharp
// 11.4.1 URL安全校验
public class UrlSecurityValidator
{
    private static readonly string[] BlockedPatterns = new[]
    {
        "localhost",
        "127.0.0.1",
        "10.",
        "172.16.", "172.17.", "172.18.", "172.19.",
        "172.20.", "172.21.", "172.22.", "172.23.",
        "172.24.", "172.25.", "172.26.", "172.27.",
        "172.28.", "172.29.", "172.30.", "172.31.",
        "192.168."
    };

    public ValidationResult Validate(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
            return ValidationResult.Success();

        if (!url.StartsWith("http://", StringComparison.OrdinalIgnoreCase) &&
            !url.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
        {
            return ValidationResult.Error("请输入有效的http或https链接");
        }

        if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
        {
            return ValidationResult.Error("请输入有效的URL地址");
        }

        var host = uri.Host.ToLowerInvariant();
        
        foreach (var pattern in BlockedPatterns)
        {
            if (host.StartsWith(pattern) || host == pattern)
            {
                return ValidationResult.Error("不支持内网地址");
            }
        }

        return ValidationResult.Success();
    }
}

// 11.4.2 字段级别权限控制
public class FieldLevelPermissionFilter : IFieldFilter
{
    public RequirementDto ApplyPermissions(Requirement requirement, User currentUser)
    {
        var dto = MapToDto(requirement);

        // 非管理员隐藏报价
        if (currentUser.Role != UserRole.Admin)
        {
            dto.Price = null;
            dto.PriceDisplay = "***";
        }

        return dto;
    }
}
```

### 11.5 兼容性要求

| 兼容性项 | 要求 | 实现方案 |
|---------|------|----------|
| **浏览器** | Chrome/Firefox/Safari/Edge最新两个版本 | 响应式设计 + 标准API |
| **移动端** | 响应式布局支持 | Tailwind CSS |
| **API版本** | v1版本支持，后续版本共存 | URL版本化 /api/v1/ |
| **数据库** | SQL Server 2019+ / PostgreSQL 14+ | EF Core配置化 |

### 11.6 数据一致性要求

| 场景 | 一致性要求 | 实现方案 | 测试用例 |
|------|----------|----------|---------|
| **状态流转** | 线性状态机 | 领域模型校验 | TC-FLOW-009~TC-FLOW-011 |
| **并发编辑** | 乐观锁 | Version字段校验 | TC-REQ-050~TC-REQ-053 |
| **时间字段** | 自动填充 | 后端触发器模式 | TC-FLOW-017~TC-FLOW-018 |
| **关联删除** | 级联约束 | 外键 + 应用层校验 | TC-BOT-008, TC-PROJ-005 |
| **事务** | ACID | EF Core事务管理 | - |

---

## 12. API详细设计

### 12.1 统一响应格式

```csharp
// 成功响应
{
    "success": true,
    "data": { ... },
    "message": null,
    "errors": null,
    "timestamp": "2025-06-01T10:00:00Z"
}

// 错误响应
{
    "success": false,
    "data": null,
    "message": "操作失败",
    "errors": [
        {
            "field": "Name",
            "message": "需求名称不能超过100个字符"
        }
    ],
    "timestamp": "2025-06-01T10:00:00Z",
    "errorCode": "VALIDATION_ERROR"
}

// 分页响应
{
    "success": true,
    "data": {
        "items": [...],
        "totalCount": 100,
        "pageIndex": 1,
        "pageSize": 20,
        "totalPages": 5
    },
    "message": null,
    "errors": null,
    "timestamp": "2025-06-01T10:00:00Z"
}
```

### 12.2 错误码定义

| 错误码 | HTTP状态码 | 说明 |
|--------|-----------|------|
| VALIDATION_ERROR | 400 | 参数校验失败 |
| UNAUTHORIZED | 401 | 未认证 |
| FORBIDDEN | 403 | 无权限 |
| NOT_FOUND | 404 | 资源不存在 |
| DUPLICATE_ERROR | 409 | 资源重复（如需求号已存在） |
| VERSION_CONFLICT | 409 | 并发版本冲突 |
| STATE_TRANSITION_INVALID | 400 | 状态流转不合法 |
| INTERNAL_ERROR | 500 | 内部错误 |

### 12.3 需求管理API详细设计

#### 12.3.1 获取需求列表

```http
GET /api/requirements
```

**Query Parameters**:
| 参数 | 类型 | 必填 | 默认值 | 说明 |
|------|------|------|--------|------|
| status | string | 否 | - | 状态筛选，多个用逗号分隔 |
| followerId | int | 否 | - | 跟进人ID |
| projectId | int | 否 | - | 项目ID |
| planStartDateFrom | date | 否 | - | 计划开始时间起 |
| planStartDateTo | date | 否 | - | 计划开始时间止 |
| keyword | string | 否 | - | 关键词搜索（名称/需求号） |
| pageIndex | int | 否 | 1 | 页码 |
| pageSize | int | 否 | 20 | 每页条数（支持10/20/50） |
| sortBy | string | 否 | createdAt | 排序字段 |
| sortOrder | string | 否 | desc | 排序方向（asc/desc） |

**Response**: 见统一响应格式，分页数据

#### 12.3.2 获取需求详情

```http
GET /api/requirements/{id}
```

**Path Parameters**:
| 参数 | 类型 | 说明 |
|------|------|------|
| id | int | 需求ID |

**Response**:
```json
{
    "id": 1,
    "name": "需求A",
    "requirementNo": "REQ-2025-001",
    "status": "PendingConfirm",
    "statusName": "待确认",
    "progress": 0,
    "followerId": 1,
    "followerName": "张三",
    "projectId": 1,
    "projectName": "项目A",
    "planStartDate": "2025-06-01",
    "planTestDate": "2025-06-15",
    "planLaunchDate": "2025-06-30",
    "actualTestDate": null,
    "actualLaunchDate": null,
    "isConfirmed": false,
    "docUrl": "https://docs.example.com/req1",
    "price": null,
    "priority": "High",
    "remark": null,
    "version": 1,
    "createdAt": "2025-06-01T10:00:00Z",
    "updatedAt": "2025-06-01T10:00:00Z",
    "availableTransitions": ["Confirmed"]  // 当前状态可转换到的下一个状态
}
```

#### 12.3.3 创建需求

```http
POST /api/requirements
```

**Request Body**:
```json
{
    "name": "需求A",
    "requirementNo": "REQ-2025-001",
    "status": "PendingConfirm",
    "progress": 0,
    "followerId": 1,
    "projectId": 1,
    "planStartDate": "2025-06-01",
    "planTestDate": "2025-06-15",
    "planLaunchDate": "2025-06-30",
    "docUrl": "https://docs.example.com/req1",
    "price": 10000.00,
    "priority": "High",
    "remark": "备注信息",
    "robotIds": [1, 2]
}
```

**Response**: 返回创建的需求详情，包含生成的ID

#### 12.3.4 更新需求

```http
PUT /api/requirements/{id}
```

**Path Parameters**:
| 参数 | 类型 | 说明 |
|------|------|------|
| id | int | 需求ID |

**Request Body**:
```json
{
    "name": "需求A（修改版）",
    "requirementNo": "REQ-2025-001",
    "status": "Confirmed",
    "progress": 0,
    "followerId": 1,
    "projectId": 1,
    "planStartDate": "2025-06-01",
    "planTestDate": "2025-06-15",
    "planLaunchDate": "2025-06-30",
    "docUrl": "https://docs.example.com/req1",
    "price": 10000.00,
    "priority": "High",
    "remark": "备注信息",
    "robotIds": [1, 2],
    "version": 1
}
```

**成功响应**: 返回更新后的需求详情

**版本冲突响应**:
```json
{
    "success": false,
    "message": "数据已被他人修改，请刷新页面获取最新数据后重新编辑",
    "errorCode": "VERSION_CONFLICT",
    "data": {
        "currentVersion": 2,
        "yourVersion": 1
    }
}
```

#### 12.3.5 更新需求状态

```http
PUT /api/requirements/{id}/status
```

**Request Body**:
```json
{
    "newStatus": "Confirmed",
    "version": 1
}
```

**成功响应**:
```json
{
    "success": true,
    "data": {
        "id": 1,
        "status": "Confirmed",
        "statusName": "已确认",
        "isConfirmed": true,
        "version": 2
    }
}
```

**非法流转响应**:
```json
{
    "success": false,
    "message": "状态变更不合法，当前状态无法转换到目标状态",
    "errorCode": "STATE_TRANSITION_INVALID"
}
```

#### 12.3.6 删除需求

```http
DELETE /api/requirements/{id}
```

**Path Parameters**:
| 参数 | 类型 | 说明 |
|------|------|------|
| id | int | 需求ID |

**成功响应**: HTTP 204 No Content

### 12.4 机器人配置API详细设计

#### 12.4.1 测试机器人连接

```http
POST /api/robots/{id}/test
```

**Response**:
```json
{
    "success": true,
    "data": {
        "connected": true,
        "responseTime": 150,
        "message": "连接测试成功"
    }
}
```

**失败响应**:
```json
{
    "success": false,
    "message": "Webhook地址无效，请检查",
    "errorCode": "VALIDATION_ERROR"
}
```

### 12.5 通知日志API详细设计

#### 12.5.1 获取通知统计

```http
GET /api/notifications/stats
```

**Query Parameters**:
| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| startTime | date | 否 | 统计开始时间 |
| endTime | date | 否 | 统计结束时间 |

**Response**:
```json
{
    "success": true,
    "data": {
        "totalSent": 1000,
        "successCount": 980,
        "failedCount": 20,
        "successRate": 98.0,
        "averageDelaySeconds": 5.2,
        "byType": {
            "StatusChange": { "total": 800, "success": 790, "failed": 10 },
            "DailyReminder": { "total": 150, "success": 150, "failed": 0 },
            "DelayWarning": { "total": 50, "success": 40, "failed": 10 }
        }
    }
}
```

---

## 13. 异常处理规范

### 13.1 异常分类

| 异常类型 | 基类 | HTTP状态码 | 处理方式 |
|----------|------|-----------|----------|
| **业务异常** | BusinessException | 400/409 | 返回友好错误信息 |
| **认证异常** | UnauthorizedException | 401 | 跳转登录 |
| **权限异常** | ForbiddenException | 403 | 提示无权限 |
| **资源不存在** | NotFoundException | 404 | 提示资源不存在 |
| **系统异常** | SystemException | 500 | 记录日志，返回通用错误 |

### 13.2 全局异常处理

```csharp
public class ExceptionHandlingMiddleware
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var response = context.Response;
        response.ContentType = "application/json";

        var errorResponse = new ErrorResponse
        {
            Timestamp = DateTime.UtcNow
        };

        switch (exception)
        {
            case BusinessException businessEx:
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                errorResponse.ErrorCode = businessEx.ErrorCode;
                errorResponse.Message = businessEx.Message;
                errorResponse.Errors = businessEx.Errors;
                break;

            case UnauthorizedException:
                response.StatusCode = (int)HttpStatusCode.Unauthorized;
                errorResponse.ErrorCode = "UNAUTHORIZED";
                errorResponse.Message = "请先登录";
                break;

            case ForbiddenException:
                response.StatusCode = (int)HttpStatusCode.Forbidden;
                errorResponse.ErrorCode = "FORBIDDEN";
                errorResponse.Message = "无权限执行此操作";
                break;

            case NotFoundException:
                response.StatusCode = (int)HttpStatusCode.NotFound;
                errorResponse.ErrorCode = "NOT_FOUND";
                errorResponse.Message = "资源不存在";
                break;

            case VersionConflictException:
                response.StatusCode = (int)HttpStatusCode.Conflict;
                errorResponse.ErrorCode = "VERSION_CONFLICT";
                errorResponse.Message = exception.Message;
                errorResponse.Data = ((VersionConflictException)exception).CurrentVersion;
                break;

            default:
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                errorResponse.ErrorCode = "INTERNAL_ERROR";
                errorResponse.Message = "系统异常，请稍后重试";
                _logger.LogError(exception, "未处理的系统异常");
                break;
        }

        await response.WriteAsJsonAsync(errorResponse);
    }
}
```

### 13.3 日志规范

```csharp
public class LoggingPolicy
{
    // 记录操作日志
    public async Task LogOperationAsync(string operation, object details)
    {
        _logger.LogInformation(
            "操作: {Operation} | 用户: {UserId} | 时间: {Timestamp} | 详情: {@Details}",
            operation,
            _currentUser.GetUserId(),
            DateTime.UtcNow,
            details);
    }

    // 记录安全日志
    public async Task LogSecurityEventAsync(string eventType, string message)
    {
        _logger.LogWarning(
            "安全事件: {EventType} | 用户: {UserId} | IP: {IP} | 时间: {Timestamp} | 消息: {Message}",
            eventType,
            _currentUser.GetUserId(),
            _httpContext.Connection.RemoteIpAddress,
            DateTime.UtcNow,
            message);
    }

    // 记录错误日志
    public async Task LogErrorAsync(Exception ex, string context)
    {
        _logger.LogError(ex,
            "错误上下文: {Context} | 时间: {Timestamp} | 异常类型: {ExceptionType}",
            context,
            DateTime.UtcNow,
            ex.GetType().Name);
    }
}
```

---

## 14. 测试用例覆盖映射

### 14.1 需求管理模块（TC-REQ-xxx）

| 测试用例 | 后端实现位置 | 验证方式 |
|----------|-------------|----------|
| TC-REQ-001~013 | RequirementsController.GetRequirements | 集成测试 |
| TC-REQ-014~043 | RequirementsController.Create | 集成测试 + 单元测试 |
| TC-REQ-044~053 | RequirementsController.Update | 集成测试 + 并发测试 |
| TC-REQ-054~056 | RequirementsController.Delete | 集成测试 |

### 14.2 状态流转模块（TC-FLOW-xxx）

| 测试用例 | 后端实现位置 | 验证方式 |
|----------|-------------|----------|
| TC-FLOW-001~008 | RequirementStateMachine | 单元测试 |
| TC-FLOW-009~011 | RequirementStateMachine + 后端校验 | 集成测试 |
| TC-FLOW-012 | RequirementsController.Update | 集成测试 |
| TC-FLOW-013 | 前端+后端联合验证 | 端到端测试 |
| TC-FLOW-014 | 后端状态校验 | 集成测试 |
| TC-FLOW-015~018 | RequirementStatusService | 单元测试 |
| TC-FLOW-019~020 | 状态变更通知流程 | 端到端测试 |

### 14.3 通知管理模块（TC-NOT-xxx）

| 测试用例 | 后端实现位置 | 验证方式 |
|----------|-------------|----------|
| TC-NOT-001~005 | NotificationService.SendStatusChange | 单元测试 |
| TC-NOT-006~008 | DailyReminderScanner | 单元测试 |
| TC-NOT-009~011 | DelayWarningScanner | 单元测试 |
| TC-NOT-012~015 | NotificationRetryService | 单元测试 |
| TC-NOT-016~018 | NotificationCompensationService | 单元测试 |
| TC-NOT-019 | NotificationsController.GetStats | 集成测试 |

---

## 15. 技术债务与后续优化

### 15.1 当前版本暂不实现的功能

| 功能 | 优先级 | 说明 |
|------|--------|------|
| 数据导入/导出Excel | P2 | 可后续版本实现 |
| 需求版本历史 | P2 | 可后续版本实现 |
| Webhook签名验证 | P3 | 企业微信支持可选签名 |
| 通知模板自定义 | P3 | 当前使用固定模板 |
| 多语言支持 | P3 | 当前仅支持中文 |

### 15.2 性能优化方向

| 优化项 | 预期收益 | 实施时机 |
|--------|----------|----------|
| Redis缓存热点数据 | 列表查询提升50% | 用户量>1000后 |
| 数据库读写分离 | 写操作提升30% | 数据库压力明显后 |
| 通知队列异步化 | API响应时间降低70% | 当前版本已实现基础异步 |
| 搜索索引（Elasticsearch） | 全文搜索提升80% | 需求量大后 |

### 15.3 监控与可观测性

| 监控项 | 实现方案 | 说明 |
|--------|----------|------|
| **APM** | Application Insights / SkyWalking | 性能监控 |
| **日志** | Serilog + ELK | 结构化日志 |
| **告警** | 通知失败率>5%告警 | 及时发现问题 |

---

## 16. 用户认证模块（v1.3 新增）

> 对应测试用例：TC-AUTH-001 ~ TC-AUTH-072（约72条）

### 16.1 认证相关实体

```csharp
// 用户账号状态
public enum AccountStatus
{
    PendingActivation = 0,  // 待启用（注册后默认状态）
    Active = 1,            // 已启用
    Disabled = 2            // 已禁用
}

// 用户实体扩展（v1.3）
public class User
{
    // ... 原有字段
    
    public string PasswordHash { get; set; }          // 密码哈希（bcrypt）
    public string? PasswordSalt { get; set; }          // 盐值
    public AccountStatus AccountStatus { get; set; } // 账号状态
    public bool IsFirstLogin { get; set; }            // 首次登录标记
    public int FailedLoginCount { get; set; }         // 连续失败次数
    public DateTime? LockedUntil { get; set; }        // 锁定截止时间
    public DateTime? LastLoginAt { get; set; }        // 最后登录时间
    public string? LastLoginIp { get; set; }          // 最后登录IP
    
    public ICollection<RefreshToken> RefreshTokens { get; set; }  // Refresh Token集合
    public ICollection<LoginLog> LoginLogs { get; set; }         // 登录日志
}

// Refresh Token
public class RefreshToken
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Token { get; set; }                  // Token值
    public DateTime ExpiresAt { get; set; }           // 过期时间
    public DateTime CreatedAt { get; set; }
    public string? CreatedByIp { get; set; }           // 创建时的IP
    public bool IsRevoked { get; set; }               // 是否已撤销
    public DateTime? RevokedAt { get; set; }
    public string? ReplacedByToken { get; set; }      // 替换的Token
}

// 登录日志
public class LoginLog
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public bool IsSuccess { get; set; }               // 是否成功
    public string? FailureReason { get; set; }        // 失败原因
    public string IpAddress { get; set; }             // IP地址
    public string UserAgent { get; set; }             // 浏览器信息
    public DateTime CreatedAt { get; set; }
}

// 邮箱验证码
public class EmailVerificationCode
{
    public int Id { get; set; }
    public string Email { get; set; }
    public string Code { get; set; }                  // 6位数字验证码
    public EmailVerificationType Type { get; set; }  // 验证码类型
    public DateTime ExpiresAt { get; set; }           // 过期时间（5分钟）
    public int UsedCount { get; set; }                // 已使用次数
    public DateTime CreatedAt { get; set; }
    public int TodaySendCount { get; set; }            // 当天发送次数
}

public enum EmailVerificationType
{
    PasswordReset = 0,    // 密码重置
    EmailBinding = 1      // 邮箱绑定
}

// 操作日志（审计）
public class AuditLog
{
    public int Id { get; set; }
    public int? UserId { get; set; }
    public string UserName { get; set; }
    public AuditAction Action { get; set; }           // 操作类型
    public string EntityType { get; set; }             // 实体类型
    public int? EntityId { get; set; }                 // 实体ID
    public string Details { get; set; }               // 操作详情（JSON）
    public string IpAddress { get; set; }              // IP地址
    public DateTime CreatedAt { get; set; }
}

public enum AuditAction
{
    Login,
    Logout,
    ChangePassword,
    CreateUser,
    UpdateUser,
    DeleteUser,
    EnableUser,
    DisableUser,
    CreateRequirement,
    UpdateRequirement,
    DeleteRequirement,
    ChangeRequirementStatus
}
```

### 16.2 认证 API

| 方法 | 端点 | 描述 | 权限 |
|------|------|------|------|
| POST | /api/auth/register | 用户自主注册 | 公开 |
| POST | /api/auth/login | 用户登录 | 公开 |
| POST | /api/auth/logout | 退出登录 | 需认证 |
| POST | /api/auth/refresh | 刷新Token | 公开 |
| POST | /api/auth/change-password | 已登录用户修改密码 | 需认证 |
| POST | /api/auth/first-login-password | 首次登录强制改密 | 需认证 |
| POST | /api/auth/forgot-password | 忘记密码-发送验证码 | 公开 |
| POST | /api/auth/reset-password | 忘记密码-重置密码 | 公开 |
| GET | /api/auth/me | 获取当前用户信息 | 需认证 |

### 16.3 密码策略

```csharp
public class PasswordPolicy
{
    public const int MinLength = 8;
    public const int MaxLength = 32;
    
    // 密码复杂度规则
    public static (bool IsValid, string ErrorMessage) ValidatePassword(string password)
    {
        if (string.IsNullOrEmpty(password))
            return (false, "密码不能为空");
            
        if (password.Length < MinLength)
            return (false, $"密码至少{MinLength}位");
            
        if (password.Length > MaxLength)
            return (false, $"密码最多{MaxLength}位");
            
        if (!password.Any(char.IsUpper))
            return (false, "密码必须包含大写字母");
            
        if (!password.Any(char.IsLower))
            return (false, "密码必须包含小写字母");
            
        if (!password.Any(char.IsDigit))
            return (false, "密码必须包含数字");
            
        return (true, string.Empty);
    }
    
    // 生成初始密码
    public static string GenerateInitialPassword()
    {
        var random = new Random();
        var chars = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghjkmnpqrstuvwxyz23456789";
        return "User@" + new string(Enumerable.Range(0, 6).Select(_ => chars[random.Next(chars.Length)]).ToArray());
    }
    
    // 密码强度计算
    public static int CalculatePasswordStrength(string password)
    {
        int score = 0;
        if (password.Length >= 8) score++;
        if (password.Length >= 12) score++;
        if (password.Any(char.IsUpper)) score++;
        if (password.Any(char.IsLower)) score++;
        if (password.Any(char.IsDigit)) score++;
        if (password.Any(c => !char.IsLetterOrDigit(c))) score++;
        return Math.Min(score, 3);  // 返回1-3表示弱中强
    }
}
```

### 16.4 注册服务

```csharp
public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IEmailService _emailService;
    private readonly IPasswordHasher _passwordHasher;
    
    public async Task<Result<RegisterResponse>> RegisterAsync(RegisterRequest request)
    {
        // 1. 参数校验
        if (string.IsNullOrWhiteSpace(request.Username))
            return Result.Fail("请填写用户名");
        if (request.Username.Length < 4)
            return Result.Fail("用户名至少4位");
        if (request.Username.Length > 50)
            return Result.Fail("用户名最多50位");
        if (_passwordPolicy.ValidatePassword(request.Password).IsValid == false)
            return Result.Fail("密码至少8位，必须包含大小写字母和数字");
        if (request.Password != request.ConfirmPassword)
            return Result.Fail("两次输入密码不一致");
        if (string.IsNullOrWhiteSpace(request.RealName))
            return Result.Fail("请填写姓名");
        if (string.IsNullOrWhiteSpace(request.Phone))
            return Result.Fail("请填写手机号");
            
        // 2. 检查用户名唯一性
        if (await _userRepository.ExistsByUsernameAsync(request.Username))
            return Result.Fail("用户名已存在");
            
        // 3. 检查手机号唯一性
        if (await _userRepository.ExistsByPhoneAsync(request.Phone))
            return Result.Fail("该手机号已注册");
            
        // 4. 检查邮箱唯一性（如果提供了邮箱）
        if (!string.IsNullOrEmpty(request.Email))
        {
            if (await _userRepository.ExistsByEmailAsync(request.Email))
                return Result.Fail("该邮箱已被注册");
        }
        
        // 5. 创建用户
        var user = new User
        {
            Username = request.Username,
            RealName = request.RealName,
            Role = UserRole.Developer,  // 自主注册默认为开发人员
            Phone = request.Phone,
            Email = request.Email,
            PasswordHash = _passwordHasher.Hash(request.Password),
            AccountStatus = AccountStatus.PendingActivation,  // 待启用
            IsFirstLogin = true,
            FailedLoginCount = 0,
            IsEnabled = true,
            CreatedAt = DateTime.UtcNow
        };
        
        await _userRepository.AddAsync(user);
        
        // 6. 记录审计日志
        await _auditLogService.LogAsync(AuditAction.CreateUser, user.Id, user.Username);
        
        return Result.Success(new RegisterResponse
        {
            Message = "注册成功，请等待管理员启用账号"
        });
    }
}
```

### 16.5 登录服务

```csharp
public class AuthService : IAuthService
{
    private const int MaxFailedAttempts = 5;
    private static readonly TimeSpan LockoutDuration = TimeSpan.FromMinutes(30);
    
    public async Task<Result<LoginResponse>> LoginAsync(LoginRequest request)
    {
        // 1. 参数校验
        if (string.IsNullOrWhiteSpace(request.Username))
            return Result.Fail("请输入用户名");
        if (string.IsNullOrWhiteSpace(request.Password))
            return Result.Fail("请输入密码");
            
        // 2. 查找用户
        var user = await _userRepository.GetByUsernameAsync(request.Username);
        if (user == null)
        {
            await RecordLoginFailureAsync(null, "用户名不存在", request.IpAddress, request.UserAgent);
            return Result.Fail("用户名或密码错误");
        }
        
        // 3. 检查账号状态
        if (user.AccountStatus == AccountStatus.Disabled)
        {
            await RecordLoginFailureAsync(user.Id, "账号已禁用", request.IpAddress, request.UserAgent);
            return Result.Fail("账号已被禁用");
        }
        
        if (user.AccountStatus == AccountStatus.PendingActivation)
        {
            await RecordLoginFailureAsync(user.Id, "账号未启用", request.IpAddress, request.UserAgent);
            return Result.Fail("账号尚未启用，请联系管理员");
        }
        
        // 4. 检查账号锁定
        if (user.LockedUntil.HasValue && user.LockedUntil > DateTime.UtcNow)
        {
            var remainingMinutes = (int)Math.Ceiling((user.LockedUntil.Value - DateTime.UtcNow).TotalMinutes);
            await RecordLoginFailureAsync(user.Id, "账号已锁定", request.IpAddress, request.UserAgent);
            return Result.Fail($"账号已被锁定，请{remainingMinutes}分钟后再试");
        }
        
        // 5. 验证密码
        if (!_passwordHasher.Verify(user.PasswordHash, request.Password))
        {
            user.FailedLoginCount++;
            if (user.FailedLoginCount >= MaxFailedAttempts)
            {
                user.LockedUntil = DateTime.UtcNow.Add(LockoutDuration);
                await RecordLoginFailureAsync(user.Id, $"连续{MaxFailedAttempts}次错误，已锁定30分钟", request.IpAddress, request.UserAgent);
                return Result.Fail($"账号已被锁定，请{MaxFailedAttempts}分钟后再试");
            }
            await _userRepository.UpdateAsync(user);
            await RecordLoginFailureAsync(user.Id, "密码错误", request.IpAddress, request.UserAgent);
            return Result.Fail("用户名或密码错误");
        }
        
        // 6. 登录成功
        user.FailedLoginCount = 0;
        user.LockedUntil = null;
        user.LastLoginAt = DateTime.UtcNow;
        user.LastLoginIp = request.IpAddress;
        await _userRepository.UpdateAsync(user);
        await RecordLoginSuccessAsync(user.Id, request.IpAddress, request.UserAgent);
        
        // 7. 生成Token
        var accessToken = _tokenService.GenerateAccessToken(user);
        var refreshToken = await CreateRefreshTokenAsync(user, request.IpAddress, request.RememberMe);
        
        return Result.Success(new LoginResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken.Token,
            ExpiresAt = refreshToken.ExpiresAt,
            User = new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                RealName = user.RealName,
                Role = user.Role,
                IsFirstLogin = user.IsFirstLogin
            },
            RequirePasswordChange = user.IsFirstLogin
        });
    }
}
```

### 16.6 Token 服务

```csharp
public class TokenService : ITokenService
{
    private readonly JwtSettings _jwtSettings;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    
    public string GenerateAccessToken(User user)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.UniqueName, user.Username),
            new Claim(ClaimTypes.Role, user.Role.ToString()),
            new Claim("realName", user.RealName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };
        
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.UtcNow.AddHours(_jwtSettings.AccessTokenExpirationHours);
        
        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: expires,
            signingCredentials: credentials
        );
        
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    
    public async Task<RefreshToken> CreateRefreshTokenAsync(User user, string ipAddress, bool rememberMe)
    {
        var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        var expiresDays = rememberMe ? 30 : 7;
        
        var refreshToken = new RefreshToken
        {
            UserId = user.Id,
            Token = token,
            ExpiresAt = DateTime.UtcNow.AddDays(expiresDays),
            CreatedAt = DateTime.UtcNow,
            CreatedByIp = ipAddress,
            IsRevoked = false
        };
        
        await _refreshTokenRepository.AddAsync(refreshToken);
        return refreshToken;
    }
    
    public async Task<Result<TokenRefreshResponse>> RefreshTokenAsync(string refreshToken)
    {
        var storedToken = await _refreshTokenRepository.GetByTokenAsync(refreshToken);
        if (storedToken == null)
            return Result.Fail("无效的Refresh Token");
            
        if (storedToken.IsRevoked)
            return Result.Fail("Token已失效，请重新登录");
            
        if (storedToken.ExpiresAt < DateTime.UtcNow)
            return Result.Fail("Refresh Token已过期，请重新登录");
            
        // 生成新的Token（rotation）
        var user = await _userRepository.GetByIdAsync(storedToken.UserId);
        var newRefreshToken = await CreateRefreshTokenAsync(user, storedToken.CreatedByIp, false);
        
        // 撤销旧Token
        storedToken.IsRevoked = true;
        storedToken.RevokedAt = DateTime.UtcNow;
        storedToken.ReplacedByToken = newRefreshToken.Token;
        await _refreshTokenRepository.UpdateAsync(storedToken);
        
        return Result.Success(new TokenRefreshResponse
        {
            AccessToken = GenerateAccessToken(user),
            RefreshToken = newRefreshToken.Token,
            ExpiresAt = newRefreshToken.ExpiresAt
        });
    }
}

public class JwtSettings
{
    public string SecretKey { get; set; }
    public string Issuer { get; set; }
    public string Audience { get; set; }
    public int AccessTokenExpirationHours { get; set; } = 2;  // Access Token 2小时
}
```

### 16.7 密码找回服务

```csharp
public class PasswordResetService : IPasswordResetService
{
    private readonly IUserRepository _userRepository;
    private readonly IEmailService _emailService;
    private readonly IEmailCodeRepository _codeRepository;
    private readonly IPasswordHasher _passwordHasher;
    
    private const int CodeExpirationMinutes = 5;
    private const int MaxCodesPerDay = 10;
    private const int MinIntervalSeconds = 60;
    
    public async Task<Result> SendResetCodeAsync(string email)
    {
        // 1. 检查邮箱是否注册
        var user = await _userRepository.GetByEmailAsync(email);
        if (user == null)
            return Result.Fail("该邮箱未注册");
            
        // 2. 检查是否绑定邮箱（账号的邮箱字段不为空）
        if (string.IsNullOrEmpty(user.Email))
            return Result.Fail("该账号未绑定邮箱，请联系管理员重置密码");
            
        // 3. 检查发送频率
        var recentCode = await _codeRepository.GetLatestAsync(email, EmailVerificationType.PasswordReset);
        if (recentCode != null)
        {
            var timeSinceLastSend = DateTime.UtcNow - recentCode.CreatedAt;
            if (timeSinceLastSend.TotalSeconds < MinIntervalSeconds)
                return Result.Fail("验证码获取过于频繁，请稍后再试");
        }
        
        // 4. 检查当天发送次数
        var todayCount = await _codeRepository.GetTodayCountAsync(email, EmailVerificationType.PasswordReset);
        if (todayCount >= MaxCodesPerDay)
            return Result.Fail("验证码获取次数超限，请明天再试");
            
        // 5. 生成验证码
        var code = new Random().Next(100000, 999999).ToString();
        var emailCode = new EmailVerificationCode
        {
            Email = email,
            Code = code,
            Type = EmailVerificationType.PasswordReset,
            ExpiresAt = DateTime.UtcNow.AddMinutes(CodeExpirationMinutes),
            UsedCount = 0,
            CreatedAt = DateTime.UtcNow,
            TodaySendCount = todayCount + 1
        };
        await _codeRepository.AddAsync(emailCode);
        
        // 6. 发送邮件
        await _emailService.SendPasswordResetCodeAsync(email, code);
        
        return Result.Success();
    }
    
    public async Task<Result> ResetPasswordAsync(ResetPasswordRequest request)
    {
        // 1. 查找验证码
        var emailCode = await _codeRepository.GetValidCodeAsync(request.Email, request.Code, EmailVerificationType.PasswordReset);
        if (emailCode == null)
            return Result.Fail("验证码错误，请重新获取");
            
        // 2. 验证过期
        if (emailCode.ExpiresAt < DateTime.UtcNow)
            return Result.Fail("验证码已过期，请重新获取");
            
        // 3. 验证格式
        if (!Regex.IsMatch(request.Code, @"^\d{6}$"))
            return Result.Fail("请输入正确的6位数字验证码");
            
        // 4. 验证密码
        var (isValid, errorMsg) = PasswordPolicy.ValidatePassword(request.NewPassword);
        if (!isValid)
            return Result.Fail(errorMsg);
            
        if (request.NewPassword != request.ConfirmPassword)
            return Result.Fail("两次输入密码不一致");
            
        // 5. 更新密码
        var user = await _userRepository.GetByEmailAsync(request.Email);
        user.PasswordHash = _passwordHasher.Hash(request.NewPassword);
        user.IsFirstLogin = false;
        await _userRepository.UpdateAsync(user);
        
        // 6. 作废验证码
        emailCode.UsedCount++;
        await _codeRepository.UpdateAsync(emailCode);
        
        // 7. 撤销所有Refresh Token
        await _refreshTokenRepository.RevokeAllByUserIdAsync(user.Id);
        
        // 8. 记录审计日志
        await _auditLogService.LogAsync(AuditAction.ChangePassword, user.Id, "密码重置");
        
        return Result.Success();
    }
}
```

### 16.8 首次登录强制改密

```csharp
public async Task<Result> FirstLoginChangePasswordAsync(FirstLoginChangePasswordRequest request)
{
    var userId = _currentUser.GetUserId();
    var user = await _userRepository.GetByIdAsync(userId);
    
    // 1. 验证当前密码
    if (!_passwordHasher.Verify(user.PasswordHash, request.CurrentPassword))
        return Result.Fail("当前密码错误");
        
    // 2. 验证新密码格式
    var (isValid, errorMsg) = PasswordPolicy.ValidatePassword(request.NewPassword);
    if (!isValid)
        return Result.Fail($"新密码{errorMsg}");
        
    // 3. 验证新密码不能与当前密码相同
    if (_passwordHasher.Verify(user.PasswordHash, request.NewPassword))
        return Result.Fail("新密码不能与当前密码相同");
        
    // 4. 验证两次密码一致
    if (request.NewPassword != request.ConfirmPassword)
        return Result.Fail("两次输入密码不一致");
        
    // 5. 更新密码
    user.PasswordHash = _passwordHasher.Hash(request.NewPassword);
    user.IsFirstLogin = false;
    await _userRepository.UpdateAsync(user);
    
    // 6. 记录审计日志
    await _auditLogService.LogAsync(AuditAction.ChangePassword, user.Id, "首次登录改密");
    
    return Result.Success();
}
```

### 16.9 默认管理员初始化

```csharp
public class DefaultAdminInitializer : IHostedService
{
    public async Task InitializeAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();
        
        // 检查是否已存在用户
        if (await userRepository.AnyAsync())
            return;
            
        // 创建默认管理员
        var admin = new User
        {
            Username = "admin",
            RealName = "系统管理员",
            Role = UserRole.Admin,
            PasswordHash = _passwordHasher.Hash("Admin@123"),
            AccountStatus = AccountStatus.Active,
            IsFirstLogin = true,
            FailedLoginCount = 0,
            IsEnabled = true,
            CreatedAt = DateTime.UtcNow,
            IsDefaultAdmin = true  // 标记为默认管理员，不可删除/禁用
        };
        
        await userRepository.AddAsync(admin);
        
        _logger.LogInformation("默认管理员账号已创建：用户名={Username}", admin.Username);
    }
}
```

### 16.10 登录页面 API

```csharp
[AllowAnonymous]
[HttpGet("login-page-info")]
public async Task<ActionResult<LoginPageInfo>> GetLoginPageInfo()
{
    return Ok(new LoginPageInfo
    {
        SystemName = "需求跟踪管理系统",
        LogoUrl = _configuration["App:LogoUrl"],
        Version = _configuration["App:Version"],
        Copyright = "© 2025 XXX公司",
        AllowSelfRegistration = true  // 是否开放自主注册
    });
}
```

---

## 17. 状态流转增强（逆向流转支持）

> 对应测试用例：TC-FLOW-021 ~ TC-FLOW-032

### 17.1 增强的状态机定义

```csharp
public class RequirementStateMachine : IRequirementStateMachine
{
    // 增强定义：每个状态可流转到前一个（逆向）和后一个（正向）
    private static readonly Dictionary<RequirementStatus, (RequirementStatus? Previous, RequirementStatus? Next)> 
        ValidTransitions = new()
    {
        { RequirementStatus.PendingConfirm,    (null, RequirementStatus.Confirmed) },              // 起点，仅能正向
        { RequirementStatus.Confirmed,        (RequirementStatus.PendingConfirm, RequirementStatus.PendingQuote) },
        { RequirementStatus.PendingQuote,      (RequirementStatus.Confirmed, RequirementStatus.Quoted) },
        { RequirementStatus.Quoted,           (RequirementStatus.PendingQuote, RequirementStatus.PendingDev) },
        { RequirementStatus.PendingDev,       (RequirementStatus.Quoted, RequirementStatus.InDev) },
        { RequirementStatus.InDev,            (RequirementStatus.PendingDev, RequirementStatus.InTest) },
        { RequirementStatus.InTest,           (RequirementStatus.InDev, RequirementStatus.AcceptedPendingLaunch) },
        { RequirementStatus.AcceptedPendingLaunch, (RequirementStatus.InTest, RequirementStatus.Launched) },
        { RequirementStatus.Launched,         (RequirementStatus.AcceptedPendingLaunch, null) }   // 终态，仅能逆向
    };
    
    public bool CanTransition(RequirementStatus from, RequirementStatus to)
    {
        if (!ValidTransitions.TryGetValue(from, out var transitions))
            return false;
            
        return transitions.Previous == to || transitions.Next == to;
    }
    
    public IEnumerable<RequirementStatus> GetAvailableTransitions(RequirementStatus current)
    {
        if (!ValidTransitions.TryGetValue(current, out var transitions))
            yield break;
            
        if (transitions.Previous.HasValue)
            yield return transitions.Previous.Value;
        if (transitions.Next.HasValue)
            yield return transitions.Next.Value;
    }
    
    public bool IsTerminalStatus(RequirementStatus status)
    {
        return status == RequirementStatus.Launched;
    }
    
    public bool IsReverseTransition(RequirementStatus from, RequirementStatus to)
    {
        if (!ValidTransitions.TryGetValue(from, out var transitions))
            return false;
        return transitions.Previous == to;
    }
}
```

### 17.2 终态特殊处理

```csharp
public class RequirementUpdateService : IRequirementUpdateService
{
    public async Task<Result> UpdateRequirementAsync(int id, UpdateRequirementDto dto)
    {
        var requirement = await _requirementRepository.GetByIdAsync(id);
        
        // 终态需求：仅允许修改备注
        if (_stateMachine.IsTerminalStatus(requirement.Status))
        {
            if (HasNonRemarkChanges(dto))
                return Result.Fail("已上线需求不可修改");
                
            requirement.Remark = dto.Remark;
            await _requirementRepository.UpdateAsync(requirement);
            return Result.Success(requirement);
        }
        
        // 非终态：执行完整更新逻辑
        // ... 原有逻辑
    }
    
    public async Task<Result> DeleteRequirementAsync(int id)
    {
        var requirement = await _requirementRepository.GetByIdAsync(id);
        
        if (_stateMachine.IsTerminalStatus(requirement.Status))
            return Result.Fail("已上线需求不可删除");
            
        await _requirementRepository.DeleteAsync(id);
        return Result.Success();
    }
}
```

### 17.3 状态变更自动逻辑（增强）

```csharp
public class RequirementStatusService : IRequirementStatusService
{
    public async Task ApplyStatusChangeAsync(Requirement requirement, RequirementStatus newStatus)
    {
        var oldStatus = requirement.Status;
        requirement.Status = newStatus;
        
        // 自动填充时间字段
        if (newStatus == RequirementStatus.InTest)
            requirement.ActualTestDate = DateTime.UtcNow;
        if (newStatus == RequirementStatus.Launched)
            requirement.ActualLaunchDate = DateTime.UtcNow;
            
        // 自动联动IsConfirmed
        if (newStatus == RequirementStatus.PendingConfirm)
            requirement.IsConfirmed = false;
        if (newStatus >= RequirementStatus.Confirmed)
            requirement.IsConfirmed = true;
    }
}
```

---

## 18. 通知系统详细实现

> 对应测试用例：TC-NOTIFY-001 ~ TC-NOTIFY-019

### 18.1 通知重试服务

```csharp
public class NotificationRetryService : BackgroundService
{
    private static readonly int[] RetryIntervalsSeconds = { 30, 120, 300 }; // 30s, 2min, 5min
    private const int MaxRetryCount = 3;
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await ProcessFailedNotificationsAsync();
            await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
        }
    }
    
    private async Task ProcessFailedNotificationsAsync()
    {
        var failedLogs = await _notificationLogRepository.GetFailedPendingRetryAsync();
        
        foreach (var log in failedLogs)
        {
            if (log.RetryCount >= MaxRetryCount)
                continue;
                
            var delay = RetryIntervalsSeconds[log.RetryCount];
            var elapsed = DateTime.UtcNow - log.LastAttemptAt;
            
            if (elapsed.TotalSeconds >= delay)
            {
                await RetrySendNotificationAsync(log);
            }
        }
    }
    
    private async Task RetrySendNotificationAsync(NotificationLog log)
    {
        log.RetryCount++;
        log.LastAttemptAt = DateTime.UtcNow;
        
        var result = await _notificationService.SendAsync(log);
        
        if (result.IsSuccess)
        {
            log.Status = NotificationStatus.Success;
            log.ErrorMessage = null;
        }
        else
        {
            log.ErrorMessage = result.ErrorMessage;
            if (log.RetryCount >= MaxRetryCount)
                log.Status = NotificationStatus.Failed;
        }
        
        await _notificationLogRepository.UpdateAsync(log);
    }
}
```

### 18.2 每日补偿扫描服务

```csharp
public class NotificationCompensationService : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var now = DateTime.UtcNow;
            var next9AM = GetNext9AM(now);
            
            var delay = next9AM - now;
            if (delay > TimeSpan.Zero)
            {
                await Task.Delay(delay, stoppingToken);
                await ScanAndCompensateAsync();
            }
            else
            {
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }
    }
    
    private async Task ScanAndCompensateAsync()
    {
        // 扫描过去24小时内失败的、不再重试的通知
        var failedNotifications = await _notificationLogRepository
            .GetFailedForCompensationAsync(TimeSpan.FromHours(24));
            
        foreach (var notification in failedNotifications)
        {
            if (notification.RetryCount >= 3)
            {
                // 再次尝试发送1次
                var result = await _notificationService.SendAsync(notification);
                notification.Status = result.IsSuccess 
                    ? NotificationStatus.Success 
                    : NotificationStatus.Failed;
                notification.ErrorMessage = result.ErrorMessage;
                notification.IsCompensation = true;
                await _notificationLogRepository.UpdateAsync(notification);
            }
        }
    }
    
    private DateTime GetNext9AM(DateTime now)
    {
        var today9AM = new DateTime(now.Year, now.Month, now.Day, 9, 0, 0, DateTimeKind.Utc);
        return now >= today9AM ? today9AM.AddDays(1) : today9AM;
    }
}
```

### 18.3 时间提醒服务（增强）

```csharp
public class DailyReminderService : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var now = DateTime.UtcNow;
            var next9AM = GetNext9AM(now);
            
            var delay = next9AM - now;
            if (delay > TimeSpan.Zero)
            {
                await Task.Delay(delay, stoppingToken);
                await SendDailyRemindersAsync();
            }
            else
            {
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }
    }
    
    private async Task SendDailyRemindersAsync()
    {
        var requirements = await _requirementRepository.GetForReminderAsync();
        var today = DateTime.UtcNow.Date;
        
        foreach (var req in requirements)
        {
            // 检查计划交测时间提醒
            if (req.PlanTestDate.HasValue && req.Status < RequirementStatus.InTest)
            {
                var daysUntil = (req.PlanTestDate.Value.Date - today).Days;
                var reminderDays = GetReminderDays(req.Priority, ReminderType.Test);
                
                if (reminderDays.Contains(daysUntil))
                {
                    await _notificationService.SendReminderAsync(req, ReminderType.Test, daysUntil);
                }
            }
            
            // 检查计划上线时间提醒
            if (req.PlanLaunchDate.HasValue && req.Status < RequirementStatus.Launched)
            {
                var daysUntil = (req.PlanLaunchDate.Value.Date - today).Days;
                var reminderDays = GetReminderDays(req.Priority, ReminderType.Launch);
                
                if (reminderDays.Contains(daysUntil))
                {
                    await _notificationService.SendReminderAsync(req, ReminderType.Launch, daysUntil);
                }
            }
        }
    }
    
    private int[] GetReminderDays(Priority priority, ReminderType type)
    {
        return (priority, type) switch
        {
            (Priority.High, ReminderType.Test) => new[] { 3, 1, 0 },
            (Priority.High, ReminderType.Launch) => new[] { 3, 1, 0 },
            (Priority.Medium, ReminderType.Test) => new[] { 2, 0 },
            (Priority.Medium, ReminderType.Launch) => new[] { 2, 0 },
            (Priority.Low, ReminderType.Test) => new[] { 1 },
            (Priority.Low, ReminderType.Launch) => new[] { 1 },
            _ => Array.Empty<int>()
        };
    }
}

public enum ReminderType
{
    Test,
    Launch
}
```

---

## 19. 增强的权限控制

> 对应测试用例：TC-NFR-002 ~ TC-NFR-005

### 19.1 完整的权限策略

```csharp
public class PermissionPolicy
{
    public static readonly Dictionary<(UserRole Role, string Operation), bool> Matrix = new()
    {
        // 需求管理
        { (UserRole.Admin, "Requirement.Create"), true },
        { (UserRole.Developer, "Requirement.Create"), false },
        { (UserRole.Tester, "Requirement.Create"), false },
        
        { (UserRole.Admin, "Requirement.Read"), true },
        { (UserRole.Developer, "Requirement.Read"), true },
        { (UserRole.Tester, "Requirement.Read"), true },
        
        { (UserRole.Admin, "Requirement.Update"), true },
        { (UserRole.Developer, "Requirement.Update.Own"), true },
        { (UserRole.Tester, "Requirement.Update.Own"), true },
        
        { (UserRole.Admin, "Requirement.Delete"), true },
        { (UserRole.Developer, "Requirement.Delete"), false },
        { (UserRole.Tester, "Requirement.Delete"), false },
        
        { (UserRole.Admin, "Requirement.ChangeStatus"), true },
        { (UserRole.Developer, "Requirement.ChangeStatus"), false },
        { (UserRole.Tester, "Requirement.ChangeStatus"), false },
        
        { (UserRole.Admin, "Requirement.ViewPrice"), true },
        { (UserRole.Developer, "Requirement.ViewPrice"), false },
        { (UserRole.Tester, "Requirement.ViewPrice"), false },
        
        // 项目管理
        { (UserRole.Admin, "Project.Create"), true },
        { (UserRole.Admin, "Project.Read"), true },
        { (UserRole.Admin, "Project.Update"), true },
        { (UserRole.Admin, "Project.Delete"), true },
        { (UserRole.Developer, "Project.*"), false },
        { (UserRole.Tester, "Project.*"), false },
        
        // 机器人配置
        { (UserRole.Admin, "Robot.*"), true },
        { (UserRole.Developer, "Robot.*"), false },
        { (UserRole.Tester, "Robot.*"), false },
        
        // 用户管理
        { (UserRole.Admin, "User.*"), true },
        { (UserRole.Developer, "User.*"), false },
        { (UserRole.Tester, "User.*"), false },
        
        // 通知日志
        { (UserRole.Admin, "Notification.Read"), true },
        { (UserRole.Developer, "Notification.Read"), false },
        { (UserRole.Tester, "Notification.Read"), false },
    };
}
```

### 19.2 默认管理员保护

```csharp
public class UserAuthorizationHandler : AuthorizationHandler<OperationRequirement, User>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        OperationRequirement requirement,
        User resource)
    {
        var currentUser = _currentUser.GetCurrentUser();
        
        switch (requirement.Operation)
        {
            case Operation.Delete:
                // 默认管理员不可删除
                if (resource.IsDefaultAdmin)
                {
                    _logger.LogWarning("尝试删除默认管理员账户: {UserId}", resource.Id);
                    return Task.CompletedTask;
                }
                break;
                
            case Operation.Disable:
                // 默认管理员不可禁用
                if (resource.IsDefaultAdmin)
                {
                    _logger.LogWarning("尝试禁用默认管理员账户: {UserId}", resource.Id);
                    return Task.CompletedTask;
                }
                break;
        }
        
        context.Succeed(requirement);
        return Task.CompletedTask;
    }
}
```

---

## 20. 数据校验规则（完整版）

> 覆盖所有测试用例的边界值校验

### 20.1 需求字段校验

```csharp
public class RequirementValidator : AbstractValidator<CreateRequirementDto>
{
    public RequirementValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("请填写需求名称")
            .MaximumLength(100).WithMessage("需求名称最多100字");
            
        RuleFor(x => x.RequirementNo)
            .NotEmpty().WithMessage("请填写需求号")
            .MaximumLength(50).WithMessage("需求号最多50字");
            
        RuleFor(x => x.Progress)
            .InclusiveBetween(0, 100).WithMessage("进度值必须在0-100之间");
            
        RuleFor(x => x.Remark)
            .MaximumLength(500).WithMessage("备注最多500字");
            
        RuleFor(x => x.Price)
            .GreaterThanOrEqualTo(0).WithMessage("报价不能为负数")
            .Must(x => decimal.Round(x ?? 0, 2) == (x ?? 0))
            .WithMessage("报价最多保留2位小数");
            
        RuleFor(x => x.DocUrl)
            .Must(BeValidPublicUrl).WithMessage("请输入有效的http或https链接，且不支持内网地址")
            .When(x => !string.IsNullOrEmpty(x.DocUrl));
            
        RuleFor(x => x)
            .Must(HaveValidDateSequence)
            .WithMessage("时间设置不合理");
    }
    
    private bool BeValidPublicUrl(string? url)
    {
        if (string.IsNullOrEmpty(url)) return true;
        
        if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
            return false;
            
        if (uri.Scheme != "http" && uri.Scheme != "https")
            return false;
            
        // 排除内网地址
        var host = uri.Host.ToLower();
        if (host == "localhost" || host == "127.0.0.1")
            return false;
            
        if (IPAddress.TryParse(host, out var ip))
        {
            if (IsPrivateIp(ip))
                return false;
        }
        
        return true;
    }
    
    private bool IsPrivateIp(IPAddress ip)
    {
        var bytes = ip.GetAddressBytes();
        // 10.0.0.0/8, 172.16.0.0/12, 192.168.0.0/16
        if (bytes[0] == 10) return true;
        if (bytes[0] == 172 && bytes[1] >= 16 && bytes[1] <= 31) return true;
        if (bytes[0] == 192 && bytes[1] == 168) return true;
        return false;
    }
    
    private bool HaveValidDateSequence(CreateRequirementDto dto)
    {
        if (dto.PlanStartDate.HasValue && dto.PlanTestDate.HasValue)
            if (dto.PlanTestDate < dto.PlanStartDate)
                return false;
                
        if (dto.PlanTestDate.HasValue && dto.PlanLaunchDate.HasValue)
            if (dto.PlanLaunchDate < dto.PlanTestDate)
                return false;
                
        return true;
    }
}
```

### 20.2 用户字段校验

```csharp
public class UserValidator : AbstractValidator<CreateUserDto>
{
    public UserValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("请填写用户名")
            .MinimumLength(4).WithMessage("用户名至少4位")
            .MaximumLength(50).WithMessage("用户名最多50位")
            .Matches("^[a-zA-Z0-9_]+$").WithMessage("用户名只能包含字母、数字和下划线");
            
        RuleFor(x => x.RealName)
            .NotEmpty().WithMessage("请填写姓名")
            .MaximumLength(50).WithMessage("姓名最多50字");
            
        RuleFor(x => x.Phone)
            .NotEmpty().WithMessage("请填写手机号")
            .Matches("^1[3-9]\\d{9}$").WithMessage("请输入有效的手机号");
            
        RuleFor(x => x.Email)
            .EmailAddress().WithMessage("请输入有效的邮箱地址")
            .When(x => !string.IsNullOrEmpty(x.Email));
    }
}
```

### 20.3 机器人字段校验

```csharp
public class RobotValidator : AbstractValidator<CreateRobotDto>
{
    public RobotValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("请填写机器人名称")
            .MaximumLength(50).WithMessage("机器人名称最多50字");
            
        RuleFor(x => x.WebhookUrl)
            .NotEmpty().WithMessage("请填写Webhook地址")
            .Must(BeValidHttpsUrl).WithMessage("Webhook地址必须为HTTPS")
            .When(x => !string.IsNullOrEmpty(x.WebhookUrl));
    }
    
    private bool BeValidHttpsUrl(string url)
    {
        if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
            return false;
        return uri.Scheme == "https";
    }
}
```

### 20.4 项目字段校验

```csharp
public class ProjectValidator : AbstractValidator<CreateProjectDto>
{
    public ProjectValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("请填写项目名称")
            .MaximumLength(100).WithMessage("项目名称最多100字");
            
        RuleFor(x => x.Code)
            .MaximumLength(50).WithMessage("项目编码最多50字");
    }
}
```

---

## 21. 测试用例覆盖映射（补充）

### 21.1 用户认证模块（TC-AUTH-xxx）

| 测试用例 | 后端实现位置 | 验证方式 |
|----------|-------------|----------|
| TC-AUTH-001~020 | AuthService.Register | 集成测试 |
| TC-AUTH-021~028 | AuthService.Login | 集成测试 |
| TC-AUTH-029~035 | AuthService.FirstLoginChangePassword | 集成测试 |
| TC-AUTH-036~049 | PasswordResetService | 集成测试 |
| TC-AUTH-045~047 | AuthService.ChangePassword | 集成测试 |
| TC-AUTH-048~053 | TokenService | 集成测试 |
| TC-AUTH-054~060 | LoginAttemptTracking + AuditLog | 集成测试 |
| TC-AUTH-061~065 | DefaultAdminInitializer | 集成测试 |
| TC-AUTH-066~072 | 登录页API + 前端 | UI测试 |

### 21.2 状态流转增强（TC-FLOW-xxx）

| 测试用例 | 后端实现位置 | 验证方式 |
|----------|-------------|----------|
| TC-FLOW-021~023 | RequirementStateMachine + UpdateService | 集成测试 |
| TC-FLOW-024 | RequirementStatusService | 单元测试 |
| TC-FLOW-025 | 全链路集成测试 | 端到端测试 |
| TC-FLOW-026~032 | RequirementStateMachine | 集成测试 |

### 21.3 通知系统（TC-NOTIFY-xxx）

| 测试用例 | 后端实现位置 | 验证方式 |
|----------|-------------|----------|
| TC-NOTIFY-001~005 | NotificationService | 单元测试 |
| TC-NOTIFY-006~012 | DailyReminderService | 单元测试 |
| TC-NOTIFY-013~015 | NotificationLogService | 集成测试 |
| TC-NOTIFY-016~018 | NotificationRetryService | 单元测试 |
| TC-NOTIFY-019 | NotificationCompensationService | 集成测试 |

### 21.4 非功能需求（TC-NFR-xxx）

| 测试用例 | 后端实现位置 | 验证方式 |
|----------|-------------|----------|
| TC-NFR-001,007 | 数据库索引 + 查询优化 | 性能测试 |
| TC-NFR-002,003 | JWT认证 + 权限策略 | 安全测试 |
| TC-NFR-004,005 | 跨浏览器测试 | UI测试 |
| TC-NFR-006 | 监控告警 | 运维验证 |
| TC-NFR-008 | BCrypt加密 | 安全测试 |
| TC-NFR-009 | AuditLog | 集成测试 |

---

## 22. 补充的实体关系

### 22.1 完整的实体关系图

```
┌─────────────┐     ┌─────────────┐     ┌─────────────┐
│    User     │     │   Project   │     │   Robot     │
├─────────────┤     ├─────────────┤     ├─────────────┤
│ Id          │     │ Id          │     │ Id          │
│ Username    │     │ Name        │     │ Name        │
│ RealName    │     │ Code        │     │ WebhookUrl  │
│ Role        │     │ ManagerId   │     │ GroupName   │
│ Phone       │     │ Description │     │ IsEnabled   │
│ Email       │     │ CreatedAt   │     │ CreatedAt   │
│ PasswordHash│     └──────┬──────┘     └──────┬──────┘
│ AccountStatus│           │                   │
│ IsFirstLogin │           │                   │
│ FailedLoginCount│        │                   │
│ LockedUntil  │           │                   │
│ LastLoginAt  │      ┌────┴───────────────────┘
│ LastLoginIp  │      │        1:N
└──────┬──────┘      ↓
       │    ┌─────────────────┐         ┌───────────────┐
       │    │   Requirement   │─────────│NotificationLog│
       │    ├─────────────────┤   1:N   ├───────────────┤
       │    │ Id              │         │ Id            │
       │    │ ...            │         │ RequirementId │
       │    │ Version        │         │ Type          │
       │    └─────────────────┘         │ RobotId       │
       │                                 │ Status        │
       │    ┌─────────────────┐         │ RetryCount    │
       │    │ RefreshToken    │         │ LastAttemptAt │
       │    ├─────────────────┤         │ IsCompensation│
       │    │ Id              │         │ SentAt        │
       │    │ UserId          │         └───────────────┘
       │    │ Token           │
       │    │ ExpiresAt      │
       │    │ IsRevoked       │         ┌───────────────┐
       │    └─────────────────┘         │ AuditLog     │
       │                                 ├───────────────┤
       │    ┌─────────────────┐         │ Id            │
       │    │  LoginLog       │         │ UserId        │
       │    ├─────────────────┤         │ Action        │
       │    │ Id              │         │ EntityType    │
       │    │ UserId          │         │ EntityId      │
       │    │ IsSuccess       │         │ Details       │
       │    │ FailureReason   │         │ IpAddress     │
       │    │ IpAddress       │         │ CreatedAt     │
       │    │ UserAgent       │         └───────────────┘
       │    │ CreatedAt       │
       │    └─────────────────┘         ┌───────────────────┐
       │                                 │EmailVerificationCode│
       │                                 ├───────────────────┤
       │                                 │ Id                  │
       │                                 │ Email               │
       │                                 │ Code                │
       │                                 │ Type                │
       │                                 │ ExpiresAt           │
       │                                 │ TodaySendCount      │
       │                                 │ CreatedAt           │
       └─────────────────────────────────┴───────────────────┘
```

---

如需进一步细化某个模块的实现细节，请告知。