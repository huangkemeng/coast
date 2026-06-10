# 需求跟踪管理系统 - 实体和实体关系实现计划

> 本文档描述需求跟踪管理系统的完整实体设计和实现计划，涵盖所有业务模块的实体定义、关系设计和实现顺序。

---

## 文档信息

| 属性 | 内容 |
|------|------|
| 项目名称 | 需求跟踪管理系统 (Coast) |
| 文档类型 | 实现计划 - 实体设计 |
| 目标系统 | .NET 8.0 + EF Core + MySQL 8.0 |
| 文档版本 | v1.1 |
| 创建日期 | 2026-06-10 |
| 更新日期 | 2026-06-10 |
| 更新说明 | 补充完整的审计相关实体 |

---

## 1. 项目架构概述

### 1.1 技术栈

| 层级 | 技术 |
|------|------|
| 运行时 | .NET 8.0 |
| Web 框架 | ASP.NET Core Web API |
| ORM | EF Core (Pomelo.EntityFrameworkCore.MySql) |
| 数据库 | MySQL 8.0 |
| 架构模式 | Clean Architecture + CQRS |
| 认证 | JWT (bcrypt 密码加密) |

### 1.2 项目结构

```
Coast.Api/
├── src/Coast.Api/                    # Web API 入口层 (Controllers)
├── src/Coast.Api.Primary/           # 业务契约层 (Contracts/Commands/Requests)
├── src/Coast.Api.Realization/       # 业务实现层 (Handlers)
├── src/Coast.Api.Infrastructure/    # 基础设施层
│   └── DataPersistence/
│       ├── DataEntityBases/         # 实体基类接口
│       └── EfCore/Entities/         # 实体定义
└── src.tests/                       # 测试项目
```

### 1.3 现有实体基类模式

实体需实现以下接口（位于 `DataEntityBases/`）：

| 接口 | 位置 | 说明 |
|------|------|------|
| `IEntity` | IEntity.cs | 实体根接口 |
| `IHasKey<T>` | IHasKey.cs | 主键定义 |
| `ICanSoftDelete` | ICanSoftDelete.cs | 软删除接口 |
| `IHasCreatedOn` | IHasCreatedOn.cs | 创建时间 |
| `IHasCreator<T>` | IHasCreator.cs | 创建人 |
| `IHasUpdater<T>` | IHasUpdater.cs | 更新人和时间 |
| `ISortable` | ISortable.cs | 排序 |

实体实现模式：
```csharp
public class EntityName : IEfEntity<EntityName>, IHasKey<int>, ICanSoftDelete, IHasCreatedOn
{
    public EntityName()
    {
        this.InitPropertyValues();  // 初始化默认值
    }

    public static void ConfigureEntityMapping(
        EntityTypeBuilder<EntityName> builder,
        IRelationalTypeMappingSource mappingSource)
    {
        builder.AutoConfigure(mappingSource);
        // 自定义配置
    }

    public int Id { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime CreatedOn { get; set; }
}
```

---

## 2. 枚举类型定义

### 2.1 需求状态枚举 (RequirementStatus)

```csharp
public enum RequirementStatus
{
    /// <summary>待确认</summary>
    Pending = 0,
    
    /// <summary>已确认</summary>
    Confirmed = 1,
    
    /// <summary>待报价</summary>
    PendingQuote = 2,
    
    /// <summary>已报价</summary>
    Quoted = 3,
    
    /// <summary>待开发</summary>
    PendingDev = 4,
    
    /// <summary>开发中</summary>
    Developing = 5,
    
    /// <summary>测试中</summary>
    Testing = 6,
    
    /// <summary>已验收待上线</summary>
    AcceptedPendingOnline = 7,
    
    /// <summary>已上线（终态，不可流转）</summary>
    Online = 8
}
```

**状态流转规则**：
- 线性流转：只能从状态N流转到N-1（回退）或N+1（前进）
- 跨状态跳转：**不允许**
- 终态规则：`Online` 为终态，不可逆向流转

### 2.2 需求优先级枚举 (RequirementPriority)

```csharp
public enum RequirementPriority
{
    /// <summary>低优先级</summary>
    Low = 0,
    
    /// <summary>中优先级（默认）</summary>
    Medium = 1,
    
    /// <summary>高优先级</summary>
    High = 2
}
```

### 2.3 用户角色枚举 (UserRole)

```csharp
public enum UserRole
{
    /// <summary>管理员（全部权限）</summary>
    Admin = 0,
    
    /// <summary>开发人员</summary>
    Developer = 1,
    
    /// <summary>测试人员</summary>
    Tester = 2
}
```

### 2.4 用户状态枚举 (UserStatus)

```csharp
public enum UserStatus
{
    /// <summary>待启用</summary>
    Pending = 0,
    
    /// <summary>已启用</summary>
    Enabled = 1,
    
    /// <summary>已禁用</summary>
    Disabled = 2
}
```

### 2.5 通知类型枚举 (NotificationType)

```csharp
public enum NotificationType
{
    /// <summary>状态变更通知</summary>
    StatusChange = 0,
    
    /// <summary>时间提醒通知</summary>
    TimeReminder = 1
}
```

### 2.6 通知发送状态枚举 (NotificationStatus)

```csharp
public enum NotificationStatus
{
    /// <summary>待发送</summary>
    Pending = 0,
    
    /// <summary>发送成功</summary>
    Success = 1,
    
    /// <summary>发送失败</summary>
    Failed = 2
}
```

### 2.7 登录状态枚举 (LoginStatus)

```csharp
public enum LoginStatus
{
    /// <summary>成功</summary>
    Success = 0,
    
    /// <summary>密码错误</summary>
    WrongPassword = 1,
    
    /// <summary>账号不存在</summary>
    UserNotFound = 2,
    
    /// <summary>账号待启用</summary>
    Pending = 3,
    
    /// <summary>账号已禁用</summary>
    Disabled = 4,
    
    /// <summary>账号已锁定</summary>
    Locked = 5
}
```

### 2.8 操作类型枚举 (OperationType)

```csharp
public enum OperationType
{
    /// <summary>创建</summary>
    Create = 0,
    
    /// <summary>更新</summary>
    Update = 1,
    
    /// <summary>删除</summary>
    Delete = 2,
    
    /// <summary>状态变更</summary>
    StatusChange = 3,
    
    /// <summary>登录</summary>
    Login = 4,
    
    /// <summary>登出</summary>
    Logout = 5,
    
    /// <summary>密码修改</summary>
    PasswordChange = 6,
    
    /// <summary>密码重置</summary>
    PasswordReset = 7,
    
    /// <summary>启用</summary>
    Enable = 8,
    
    /// <summary>禁用</summary>
    Disable = 9,
    
    /// <summary>权限变更</summary>
    PermissionChange = 10
}
```

### 2.9 操作对象类型枚举 (TargetType)

```csharp
public enum TargetType
{
    /// <summary>需求</summary>
    Requirement = 0,
    
    /// <summary>项目</summary>
    Project = 1,
    
    /// <summary>用户</summary>
    User = 2,
    
    /// <summary>机器人</summary>
    Robot = 3,
    
    /// <summary>系统</summary>
    System = 4
}
```

### 2.10 变更字段类型枚举 (ChangeFieldType)

```csharp
public enum ChangeFieldType
{
    /// <summary>文本</summary>
    Text = 0,
    
    /// <summary>数值</summary>
    Number = 1,
    
    /// <summary>日期</summary>
    DateTime = 2,
    
    /// <summary>枚举</summary>
    Enum = 3,
    
    /// <summary>外键引用</summary>
    ForeignKey = 4,
    
    /// <summary>布尔值</summary>
    Boolean = 5
}
```

---

## 3. 实体详细设计

### 3.1 ApplicationUser（应用用户）

**文件路径**: `Coast.Api.Infrastructure/DataPersistence/EfCore/Entities/ApplicationUser.cs`

**接口实现**:
- `IEfEntity<ApplicationUser>`
- `IHasKey<Guid>`
- `ICanSoftDelete`
- `IHasCreatedOn`

**字段定义**:

| 字段名 | 类型 | 约束 | 说明 |
|--------|------|------|------|
| Id | Guid | PK | 主键 |
| Username | string(50) | Unique, Required | 用户名，登录账号 |
| PasswordHash | string(200) | Required | 密码哈希（bcrypt，强度12） |
| Name | string(100) | Required | 姓名 |
| PhoneNumber | string(20) | Unique | 手机号 |
| Email | string(100) | - | 邮箱（可选） |
| Role | UserRole | Required | 用户角色 |
| Status | UserStatus | Required | 用户状态 |
| IsFirstLogin | bool | Required | 是否首次登录 |
| PasswordErrorCount | int | Required | 密码错误次数 |
| LockedUntil | DateTime? | - | 锁定截止时间 |
| LastLoginOn | DateTime? | - | 最后登录时间 |
| IsDeleted | bool | Required | 软删除标记 |
| CreatedOn | DateTime | Required | 创建时间 |
| CreatedBy | Guid? | - | 创建人 |
| UpdatedOn | DateTime? | - | 更新时间 |
| UpdatedBy | Guid? | - | 更新人 |

**索引设计**:
- `IX_application_user_username` - Unique on Username
- `IX_application_user_phone_number` - Unique on PhoneNumber

**设计说明**:
- 使用 Guid 作为主键，与 JWT ClaimTypes.NameIdentifier 一致
- 密码使用 bcrypt 加密存储
- 首次登录标记用于强制修改初始密码
- 锁定机制：连续错误5次锁定30分钟

---

### 3.2 Requirement（需求核心实体）

**文件路径**: `Coast.Api.Infrastructure/DataPersistence/EfCore/Entities/Requirement.cs`

**接口实现**:
- `IEfEntity<Requirement>`
- `IHasKey<int>`
- `ICanSoftDelete`
- `IHasCreatedOn`

**字段定义**:

| 字段名 | 类型 | 约束 | 说明 |
|--------|------|------|------|
| Id | int | PK, Auto | 主键，自增 |
| Name | string(100) | Required | 需求名称 |
| Code | string(50) | Unique, Required | 需求号，唯一标识 |
| Status | RequirementStatus | Required | 当前状态 |
| Progress | int | - | 进度 0-100 |
| FollowerId | Guid | Required, FK | 跟进人 |
| ProjectId | int | Required, FK | 所属项目 |
| RobotId | int? | FK | 通知机器人 |
| Priority | RequirementPriority | Required | 优先级，默认 Medium |
| PlanStartDate | DateTime? | - | 计划开始时间 |
| PlanTestDate | DateTime? | - | 计划交测时间 |
| PlanOnlineDate | DateTime? | - | 计划上线时间 |
| ActualTestDate | DateTime? | - | 实际交测时间 |
| ActualOnlineDate | DateTime? | - | 实际上线时间 |
| IsConfirmed | bool | Required | 需求已确认标记（系统自动控制） |
| DocumentUrl | string(500) | - | 需求文档链接 |
| Quote | decimal(18,2) | - | 报价（仅管理员可见） |
| Remark | string(500) | - | 备注 |
| Version | int | Required | 并发控制版本号 |
| IsDeleted | bool | Required | 软删除标记 |
| CreatedOn | DateTime | Required | 创建时间 |
| CreatedBy | Guid? | - | 创建人 |
| UpdatedOn | DateTime? | - | 更新时间 |
| UpdatedBy | Guid? | - | 更新人 |

**索引设计**:
- `IX_requirement_code` - Unique on Code
- `IX_requirement_status` - on Status
- `IX_requirement_follower_id` - on FollowerId
- `IX_requirement_project_id` - on ProjectId
- `IX_requirement_robot_id` - on RobotId
- `IX_requirement_plan_test_date` - on PlanTestDate
- `IX_requirement_plan_online_date` - on PlanOnlineDate
- `IX_requirement_status_follower` - on (Status, FollowerId)
- `IX_requirement_status_project` - on (Status, ProjectId)

**业务规则**:
1. **状态自动填充**:
   - Status → Testing: ActualTestDate = 当前时间
   - Status → Online: ActualOnlineDate = 当前时间
2. **IsConfirmed 自动控制**:
   - Status ≥ Confirmed 时: IsConfirmed = true
   - Status = Pending 时: IsConfirmed = false
3. **Quote 字段权限**: 仅 Admin 角色可见和可编辑

---

### 3.3 Project（项目）

**文件路径**: `Coast.Api.Infrastructure/DataPersistence/EfCore/Entities/Project.cs`

**接口实现**:
- `IEfEntity<Project>`
- `IHasKey<int>`
- `ICanSoftDelete`
- `IHasCreatedOn`

**字段定义**:

| 字段名 | 类型 | 约束 | 说明 |
|--------|------|------|------|
| Id | int | PK, Auto | 主键，自增 |
| Name | string(100) | Required | 项目名称 |
| Code | string(50) | - | 项目编码 |
| ManagerId | Guid? | FK | 项目负责人 |
| Description | string(500) | - | 项目描述 |
| IsDeleted | bool | Required | 软删除标记 |
| CreatedOn | DateTime | Required | 创建时间 |
| CreatedBy | Guid? | - | 创建人 |
| UpdatedOn | DateTime? | - | 更新时间 |
| UpdatedBy | Guid? | - | 更新人 |

**索引设计**:
- `IX_project_name` - Unique on Name
- `IX_project_code` - on Code
- `IX_project_manager_id` - on ManagerId

**业务规则**:
- 删除项目时检查是否存在关联需求，有则阻止删除

---

### 3.4 Robot（企业微信机器人）

**文件路径**: `Coast.Api.Infrastructure/DataPersistence/EfCore/Entities/Robot.cs`

**接口实现**:
- `IEfEntity<Robot>`
- `IHasKey<int>`
- `ICanSoftDelete`
- `IHasCreatedOn`

**字段定义**:

| 字段名 | 类型 | 约束 | 说明 |
|--------|------|------|------|
| Id | int | PK, Auto | 主键，自增 |
| Name | string(50) | Required | 机器人名称 |
| WebhookUrl | string(500) | Required | Webhook 地址 |
| GroupName | string(100) | - | 所属群组 |
| IsEnabled | bool | Required | 启用状态 |
| IsDeleted | bool | Required | 软删除标记 |
| CreatedOn | DateTime | Required | 创建时间 |
| CreatedBy | Guid? | - | 创建人 |
| UpdatedOn | DateTime? | - | 更新时间 |
| UpdatedBy | Guid? | - | 更新人 |

**索引设计**:
- `IX_robot_name` - on Name
- `IX_robot_is_enabled` - on IsEnabled

**业务规则**:
- WebhookUrl 必须是 https:// 开头
- 删除机器人时：关联需求的 RobotId 置空，并记录通知日志

---

### 3.5 NotificationLog（通知日志）

**文件路径**: `Coast.Api.Infrastructure/DataPersistence/EfCore/Entities/NotificationLog.cs`

**接口实现**:
- `IEfEntity<NotificationLog>`
- `IHasKey<int>`
- `IHasCreatedOn`

**字段定义**:

| 字段名 | 类型 | 约束 | 说明 |
|--------|------|------|------|
| Id | int | PK, Auto | 主键，自增 |
| RequirementId | int | FK, Required | 关联需求 |
| RobotId | int? | FK | 接收机器人 |
| Type | NotificationType | Required | 通知类型 |
| Status | NotificationStatus | Required | 发送状态 |
| Content | string(2000) | Required | 通知内容 |
| ErrorMessage | string(500) | - | 失败原因 |
| RetryCount | int | Required | 重试次数 |
| SentAt | DateTime? | - | 发送时间 |
| CreatedOn | DateTime | Required | 创建时间 |
| CreatedBy | Guid? | - | 创建人 |

**索引设计**:
- `IX_notification_log_requirement_id` - on RequirementId
- `IX_notification_log_robot_id` - on RobotId
- `IX_notification_log_status` - on Status
- `IX_notification_log_created_on` - on CreatedOn
- `IX_notification_log_sent_at` - on SentAt

**设计说明**:
- 不实现软删除，历史记录需永久保留
- RobotId 为 nullable，机器人删除后保留日志记录

---

### 3.6 LoginLog（登录日志）

**文件路径**: `Coast.Api.Infrastructure/DataPersistence/EfCore/Entities/LoginLog.cs`

**接口实现**:
- `IEfEntity<LoginLog>`
- `IHasKey<int>`
- `IHasCreatedOn`

**字段定义**:

| 字段名 | 类型 | 约束 | 说明 |
|--------|------|------|------|
| Id | int | PK, Auto | 主键，自增 |
| UserId | Guid? | FK | 关联用户 |
| Username | string(50) | Required | 登录用户名 |
| Status | LoginStatus | Required | 登录状态 |
| IpAddress | string(50) | - | IP 地址 |
| UserAgent | string(500) | - | 浏览器 User-Agent |
| ErrorMessage | string(200) | - | 错误信息 |
| CreatedOn | DateTime | Required | 登录时间 |

**索引设计**:
- `IX_login_log_user_id` - on UserId
- `IX_login_log_username` - on Username
- `IX_login_log_created_on` - on CreatedOn

---

### 3.7 VerificationCode（短信验证码）

**文件路径**: `Coast.Api.Infrastructure/DataPersistence/EfCore/Entities/VerificationCode.cs`

**接口实现**:
- `IEfEntity<VerificationCode>`
- `IHasKey<int>`
- `ICanSoftDelete`

**字段定义**:

| 字段名 | 类型 | 约束 | 说明 |
|--------|------|------|------|
| Id | int | PK, Auto | 主键，自增 |
| PhoneNumber | string(20) | Required | 手机号 |
| Code | string(10) | Required | 验证码 |
| Type | string(20) | Required | 验证码类型 |
| ExpiresAt | DateTime | Required | 过期时间 |
| UsedAt | DateTime? | - | 使用时间 |
| UsedCount | int | Required | 使用次数 |
| SendCount | int | Required | 今日发送次数 |
| LastSentOn | DateTime | Required | 最后发送时间 |
| IsDeleted | bool | Required | 软删除标记 |
| CreatedOn | DateTime | Required | 创建时间 |

**索引设计**:
- `IX_verification_code_phone_number` - on PhoneNumber
- `IX_verification_code_code_type` - on (Code, Type)
- `IX_verification_code_expires_at` - on ExpiresAt

**业务规则**:
- 验证码规则：6位数字，有效期5分钟
- 防轰炸：每手机号每分钟最多1次，每天最多10次

---

### 3.8 OperationLog（操作日志 / 敏感操作审计）

**文件路径**: `Coast.Api.Infrastructure/DataPersistence/EfCore/Entities/OperationLog.cs`

**接口实现**:
- `IEfEntity<OperationLog>`
- `IHasKey<int>`
- `IHasCreatedOn`

**字段定义**:

| 字段名 | 类型 | 约束 | 说明 |
|--------|------|------|------|
| Id | int | PK, Auto | 主键，自增 |
| OperatorId | Guid? | FK | 操作人 |
| OperatorName | string(100) | - | 操作人姓名（冗余存储） |
| OperationType | OperationType | Required | 操作类型 |
| TargetType | TargetType | Required | 操作对象类型 |
| TargetId | string(50) | - | 操作对象ID |
| TargetName | string(200) | - | 操作对象名称 |
| IpAddress | string(50) | - | IP 地址 |
| UserAgent | string(500) | - | 浏览器 User-Agent |
| Description | string(500) | - | 操作描述 |
| ExtraData | string(2000) | - | 额外数据（JSON格式） |
| CreatedOn | DateTime | Required | 操作时间 |

**索引设计**:
- `IX_operation_log_operator_id` - on OperatorId
- `IX_operation_log_operation_type` - on OperationType
- `IX_operation_log_target` - on (TargetType, TargetId)
- `IX_operation_log_created_on` - on CreatedOn

**设计说明**:
- 不实现软删除，历史记录需永久保留
- 用于安全审计，记录密码修改、权限变更等敏感操作
- ExtraData 存储操作相关的额外信息（如修改前后的值）

**记录场景**:
| 操作类型 | TargetType | 记录内容 |
|----------|-----------|----------|
| PasswordChange | User | 密码修改成功 |
| PasswordReset | User | 密码重置 |
| PermissionChange | User | 权限/角色变更 |
| Enable/Disable | User | 用户启用/禁用 |
| Create/Update/Delete | Requirement | 需求创建/修改/删除 |
| Create/Update/Delete | Project | 项目创建/修改/删除 |
| Create/Update/Delete | Robot | 机器人创建/修改/删除 |

---

### 3.9 DataChangeLog（数据变更日志 / 字段变更明细）

**文件路径**: `Coast.Api.Infrastructure/DataPersistence/EfCore/Entities/DataChangeLog.cs`

**接口实现**:
- `IEfEntity<DataChangeLog>`
- `IHasKey<int>`
- `IHasCreatedOn`

**字段定义**:

| 字段名 | 类型 | 约束 | 说明 |
|--------|------|------|------|
| Id | int | PK, Auto | 主键，自增 |
| OperationLogId | int | FK, Required | 关联操作日志 |
| EntityType | TargetType | Required | 实体类型 |
| EntityId | string(50) | Required | 实体ID |
| FieldName | string(100) | Required | 字段名 |
| FieldDisplayName | string(100) | - | 字段显示名 |
| FieldType | ChangeFieldType | Required | 字段类型 |
| OldValue | string(500) | - | 修改前值 |
| NewValue | string(500) | - | 修改后值 |
| CreatedOn | DateTime | Required | 变更时间 |

**索引设计**:
- `IX_data_change_log_operation_log_id` - on OperationLogId
- `IX_data_change_log_entity` - on (EntityType, EntityId)
- `IX_data_change_log_field_name` - on FieldName
- `IX_data_change_log_created_on` - on CreatedOn

**设计说明**:
- 不实现软删除，历史记录需永久保留
- 关联 OperationLog，一个操作可包含多个字段变更
- 用于记录数据变更的详细信息，支持数据回溯
- OldValue/NewValue 根据 FieldType 进行序列化

**字段变更示例**:
```
Operation: 需求状态变更 (REQ-001)
├── Field: Status
│   ├── Old: 开发中
│   └── New: 测试中
└── Field: ActualTestDate
    ├── Old: (null)
    └── New: 2025-05-08 10:30:00
```

---

### 3.10 AuditSession（审计会话）

**文件路径**: `Coast.Api.Infrastructure/DataPersistence/EfCore/Entities/AuditSession.cs`

**接口实现**:
- `IEfEntity<AuditSession>`
- `IHasKey<Guid>`
- `IHasCreatedOn`

**字段定义**:

| 字段名 | 类型 | 约束 | 说明 |
|--------|------|------|------|
| Id | Guid | PK | 主键（会话ID） |
| UserId | Guid | FK, Required | 用户ID |
| IpAddress | string(50) | - | 登录IP |
| UserAgent | string(500) | - | 浏览器 User-Agent |
| RefreshToken | string(200) | - | Refresh Token（用于会话追踪） |
| LoginAt | DateTime | Required | 登录时间 |
| LastActivityAt | DateTime | Required | 最后活动时间 |
| ExpiresAt | DateTime | Required | 会话过期时间 |
| IsActive | bool | Required | 会话是否有效 |
| CreatedOn | DateTime | Required | 创建时间 |

**索引设计**:
- `IX_audit_session_user_id` - on UserId
- `IX_audit_session_login_at` - on LoginAt
- `IX_audit_session_last_activity_at` - on LastActivityAt
- `IX_audit_session_is_active` - on IsActive

**设计说明**:
- 用于会话管理和审计追踪
- 退出登录时设置 IsActive = false
- 定时清理过期会话

---

### 3.11 ApiAccessLog（API访问日志）

**文件路径**: `Coast.Api.Infrastructure/DataPersistence/EfCore/Entities/ApiAccessLog.cs`

**接口实现**:
- `IEfEntity<ApiAccessLog>`
- `IHasKey<int>`
- `IHasCreatedOn`

**字段定义**:

| 字段名 | 类型 | 约束 | 说明 |
|--------|------|------|------|
| Id | int | PK, Auto | 主键，自增 |
| SessionId | Guid? | FK | 关联会话 |
| UserId | Guid? | FK | 用户ID（可为空，匿名访问） |
| HttpMethod | string(10) | Required | HTTP方法 |
| ApiPath | string(200) | Required | API路径 |
| QueryString | string(500) | - | 查询参数 |
| RequestBody | string(2000) | - | 请求体（脱敏） |
| ResponseStatusCode | int | Required | 响应状态码 |
| ResponseTimeMs | int | Required | 响应时间（毫秒） |
| IpAddress | string(50) | - | IP 地址 |
| UserAgent | string(500) | - | 浏览器 User-Agent |
| ErrorMessage | string(1000) | - | 错误信息 |
| CreatedOn | DateTime | Required | 访问时间 |

**索引设计**:
- `IX_api_access_log_session_id` - on SessionId
- `IX_api_access_log_user_id` - on UserId
- `IX_api_access_log_api_path` - on ApiPath
- `IX_api_access_log_created_on` - on CreatedOn
- `IX_api_access_log_response_status` - on ResponseStatusCode

**设计说明**:
- 用于 API 性能监控和调用分析
- RequestBody 需脱敏处理（隐藏敏感字段）
- 可用于慢请求分析和异常追踪

---

## 4. 实体关系图

```
┌─────────────────────────────────────────────────────────────────────────────────────────────────────────┐
│                                    需求跟踪管理系统 - 实体关系图                                               │
└─────────────────────────────────────────────────────────────────────────────────────────────────────────┘

                                                 ┌─────────────────────────────────────────┐
                                                 │            ApplicationUser              │
                                                 ├─────────────────────────────────────────┤
                                                 │ Id (PK): Guid                            │
                                                 │ Username: string (Unique)               │
                                                 │ PasswordHash: string                     │
                                                 │ Name: string                             │
                                                 │ PhoneNumber: string (Unique)             │
                                                 │ Role: UserRole                           │
                                                 │ Status: UserStatus                       │
                                                 │ IsFirstLogin: bool                        │
                                                 │ PasswordErrorCount: int                  │
                                                 │ LockedUntil: DateTime?                   │
                                                 │ LastLoginOn: DateTime?                   │
                                                 │ ...                                      │
                                                 └─────────────────────────────────────────┘
                                                      │
                    ┌────────────────────────────────┼────────────────────────────────────────┐
                    │                                │                                        │
                    │ 1:N                            │ 1:N                                    │ 1:N
                    ▼                                ▼                                        ▼
    ┌─────────────────────────┐      ┌─────────────────────┐       ┌─────────────────────┐
    │       LoginLog          │      │     Requirement     │       │       Project       │
    ├─────────────────────────┤      ├─────────────────────┤       ├─────────────────────┤
    │ Id (PK): int            │      │ Id (PK): int        │       │ Id (PK): int        │
    │ UserId (FK): Guid       │◄─────│ Name: string        │       │ Name: string (Unique)│
    │ Username: string         │      │ Code: string        │       │ Code: string        │
    │ Status: LoginStatus     │      │ FollowerId (FK)─────┼───────┤ ManagerId (FK): Guid│
    │ IpAddress: string       │      │ ProjectId (FK)──────┼─┐     │ ...                 │
    │ UserAgent: string       │      │ RobotId (FK): int?  │ │     └─────────────────────┘
    │ ...                     │      │ Version: int        │ │
    └─────────────────────────┘      │ Quote: decimal      │ │            1:N
                    │                │ ...                 │ │              ▼
                    │                └─────────────────────┘ │    ┌─────────────────────┐
                    │                         ▲             │    │       Robot         │
                    │                         │ 1:N          │    ├─────────────────────┤
                    │                         └──────────────┘    │ Id (PK): int        │
    ┌─────────────────────────┐                ┌─────────────────────┐│ Name: string        │
    │    AuditSession         │                │   NotificationLog   ││ WebhookUrl: string │
    ├─────────────────────────┤                ├─────────────────────┤│ GroupName: string   │
    │ Id (PK): Guid           │                │ Id (PK): int        ││ IsEnabled: bool    │
    │ UserId (FK): Guid       │◄───────────────│ RequirementId (FK)──┘│ ...                 │
    │ IpAddress: string       │                │ RobotId (FK): int?   │└─────────────────────┘
    │ UserAgent: string       │                │ Type: NotifType      │
    │ LoginAt: DateTime       │                │ Status: NotifStatus  │
    │ LastActivityAt: DateTime│                │ Content: string      │
    │ IsActive: bool          │                │ RetryCount: int      │
    │ ...                     │                │ ...                  │
    └─────────────────────────┘                └─────────────────────┘
             (审计会话)                                   (通知日志)
                    │
                    │ 1:N
                    ▼
    ┌─────────────────────────┐      ┌─────────────────────┐
    │      OperationLog        │      │   VerificationCode  │
    ├─────────────────────────┤      ├─────────────────────┤
    │ Id (PK): int            │      │ Id (PK): int        │
    │ OperatorId (FK): Guid  │      │ PhoneNumber: string │
    │ OperatorName: string   │      │ Code: string        │
    │ OperationType: enum    │      │ Type: string        │
    │ TargetType: enum        │      │ ExpiresAt: DateTime │
    │ TargetId: string        │      │ ...                 │
    │ TargetName: string      │      └─────────────────────┘
    │ Description: string     │           (短信验证码)
    │ ExtraData: string       │
    │ ...                     │              ┌─────────────────────┐
    └─────────────────────────┘              │    DataChangeLog    │
             (操作日志)                       ├─────────────────────┤
                    │                        │ Id (PK): int        │
                    │ 1:N                    │ OperationLogId (FK) │
                    └────────────────────────►│ EntityType: enum    │
                                             │ EntityId: string    │
                                             │ FieldName: string   │
                                             │ OldValue: string    │
                                             │ NewValue: string    │
                                             │ ...                 │
                                             └─────────────────────┘
                                                  (数据变更明细)

    ┌─────────────────────────────────────────────────────────────────────────┐
    │                         ApiAccessLog (API访问日志)                        │
    ├─────────────────────────────────────────────────────────────────────────┤
    │ Id (PK): int        │ SessionId (FK): Guid? │ UserId (FK): Guid?        │
    │ HttpMethod: string  │ ApiPath: string       │ ResponseStatusCode: int   │
    │ ResponseTimeMs: int │ IpAddress: string     │ ErrorMessage: string      │
    │ CreatedOn: DateTime                                                      │
    └─────────────────────────────────────────────────────────────────────────┘
```

### 4.1 关系说明

| 关系 | 类型 | 说明 |
|------|------|------|
| User → Requirement | 1:N | 用户作为需求的跟进人 |
| Project → Requirement | 1:N | 项目包含多个需求 |
| Robot → Requirement | 1:N | 机器人可被多个需求关联 |
| User → LoginLog | 1:N | 用户有多个登录记录 |
| User → AuditSession | 1:N | 用户有多个会话 |
| Requirement → NotificationLog | 1:N | 需求产生多个通知记录 |
| Robot → NotificationLog | 1:N | 机器人发送多个通知 |
| AuditSession → OperationLog | 1:N | 会话包含多个操作 |
| OperationLog → DataChangeLog | 1:N | 操作包含多个字段变更 |

### 4.2 级联删除规则

| 父实体 | 子实体 | 删除行为 |
|--------|--------|----------|
| ApplicationUser | Requirement (FollowerId) | Restrict（阻止删除） |
| Project | Requirement | Restrict（阻止删除） |
| Robot | Requirement | SetNull（置空 RobotId） |
| OperationLog | DataChangeLog | Cascade（级联删除） |

---

## 5. 关键业务规则实现

### 5.1 需求状态流转规则

```
待确认 ──→ 已确认 ──→ 待报价 ──→ 已报价 ──→ 待开发 ──→ 开发中 ──→ 测试中 ──→ 已验收待上线 ──→ 已上线
   ▲         │                                                          │
   │         │ 逆向流转                                                  │ 终态
   └─────────┴──────────────────────────────────────────────────────────┘
              （仅允许相邻状态回退）
```

| 当前状态 | 可流转至 | 说明 |
|----------|----------|------|
| Pending | Confirmed | 需求确认 |
| Confirmed | Pending / PendingQuote | 回退/前进 |
| PendingQuote | Confirmed / Quoted | 回退/前进 |
| Quoted | PendingQuote / PendingDev | 回退/前进 |
| PendingDev | Quoted / Developing | 回退/前进 |
| Developing | PendingDev / Testing | 回退/前进 |
| Testing | Developing / AcceptedPendingOnline | 回退/前进 |
| AcceptedPendingOnline | Testing / Online | 回退/前进 |
| Online | (无) | 终态，不可流转 |

### 5.2 并发控制实现（乐观锁）

```csharp
public async Task<bool> UpdateRequirementAsync(Requirement requirement, int expectedVersion)
{
    var existing = await _dbContext.Requirements
        .FirstOrDefaultAsync(r => r.Id == requirement.Id);
    
    if (existing.Version != expectedVersion)
    {
        throw new ConcurrentModificationException(
            "数据已被他人修改，请刷新页面获取最新数据后重新编辑");
    }
    
    requirement.Version = existing.Version + 1;
    requirement.UpdatedOn = DateTime.UtcNow;
    _dbContext.Requirements.Update(requirement);
    
    return true;
}
```

### 5.3 删除约束检查

| 被删除实体 | 检查条件 | 阻止消息 |
|------------|----------|----------|
| ApplicationUser | 检查 Requirement.FollowerId | "该用户是 X 条需求的跟进人，无法删除" |
| Project | 检查 Requirement.ProjectId | "该项目下存在需求，无法删除" |
| Robot | 检查 Requirement.RobotId | 自动置空 + 记录日志 |

---

## 6. 文件创建顺序

| 顺序 | 文件路径 | 依赖 | 说明 |
|------|----------|------|------|
| 1 | `DataPersistence/EfCore/Entities/Enums/` | 无 | 所有枚举类型定义 |
| 2 | `DataPersistence/EfCore/Entities/ApplicationUser.cs` | Enums | 扩展现有用户实体 |
| 3 | `DataPersistence/EfCore/Entities/Project.cs` | 无 | 项目实体 |
| 4 | `DataPersistence/EfCore/Entities/Robot.cs` | 无 | 机器人实体 |
| 5 | `DataPersistence/EfCore/Entities/Requirement.cs` | User, Project, Robot, Enums | 需求核心实体 |
| 6 | `DataPersistence/EfCore/Entities/NotificationLog.cs` | Requirement, Robot | 通知日志 |
| 7 | `DataPersistence/EfCore/Entities/LoginLog.cs` | ApplicationUser | 登录日志 |
| 8 | `DataPersistence/EfCore/Entities/VerificationCode.cs` | 无 | 短信验证码 |
| 9 | `DataPersistence/EfCore/Entities/AuditSession.cs` | ApplicationUser | 审计会话 |
| 10 | `DataPersistence/EfCore/Entities/OperationLog.cs` | AuditSession | 操作日志 |
| 11 | `DataPersistence/EfCore/Entities/DataChangeLog.cs` | OperationLog | 数据变更日志 |
| 12 | `DataPersistence/EfCore/Entities/ApiAccessLog.cs` | AuditSession | API访问日志 |

---

## 7. 枚举文件清单

创建以下枚举文件于 `Coast.Api.Infrastructure/DataPersistence/EfCore/Entities/Enums/`：

| 文件名 | 枚举名 | 说明 |
|--------|--------|------|
| `RequirementStatus.cs` | RequirementStatus | 需求状态（9个状态） |
| `RequirementPriority.cs` | RequirementPriority | 需求优先级（3个优先级） |
| `UserRole.cs` | UserRole | 用户角色（3个角色） |
| `UserStatus.cs` | UserStatus | 用户状态（3个状态） |
| `NotificationType.cs` | NotificationType | 通知类型（2个类型） |
| `NotificationStatus.cs` | NotificationStatus | 通知发送状态（3个状态） |
| `LoginStatus.cs` | LoginStatus | 登录状态（6个状态） |
| `OperationType.cs` | OperationType | 操作类型（11种操作） |
| `TargetType.cs` | TargetType | 操作对象类型（5种类型） |
| `ChangeFieldType.cs` | ChangeFieldType | 变更字段类型（6种类型） |

---

## 8. 数据库表设计汇总

### 8.1 业务表

| 序号 | 表名 | 主键 | 外键 | 唯一约束 | 说明 |
|------|------|------|------|----------|------|
| 1 | application_user | Id (Guid) | - | Username, PhoneNumber | 应用用户 |
| 2 | project | Id (int) | ManagerId → application_user | Name | 项目 |
| 3 | robot | Id (int) | - | - | 企业微信机器人 |
| 4 | requirement | Id (int) | FollowerId → application_user<br>ProjectId → project<br>RobotId → robot | Code | 需求 |
| 5 | notification_log | Id (int) | RequirementId → requirement<br>RobotId → robot | - | 通知日志 |
| 6 | verification_code | Id (int) | - | (Code, Type) | 短信验证码 |

### 8.2 认证审计表

| 序号 | 表名 | 主键 | 外键 | 索引 | 说明 |
|------|------|------|------|------|------|
| 7 | login_log | Id (int) | UserId → application_user | UserId, Username, CreatedOn | 登录日志 |
| 8 | audit_session | Id (Guid) | UserId → application_user | UserId, LoginAt, IsActive | 审计会话 |
| 9 | api_access_log | Id (int) | SessionId → audit_session<br>UserId → application_user | SessionId, UserId, ApiPath, CreatedOn | API访问日志 |

### 8.3 操作审计表

| 序号 | 表名 | 主键 | 外键 | 索引 | 说明 |
|------|------|------|------|------|------|
| 10 | operation_log | Id (int) | OperatorId → application_user | OperatorId, OperationType, Target, CreatedOn | 操作日志 |
| 11 | data_change_log | Id (int) | OperationLogId → operation_log | OperationLogId, Entity, FieldName, CreatedOn | 数据变更明细 |

---

## 9. 实现检查清单

### 9.1 枚举实现
- [ ] RequirementStatus 枚举（9个状态）
- [ ] RequirementPriority 枚举（3个优先级）
- [ ] UserRole 枚举（3个角色）
- [ ] UserStatus 枚举（3个状态）
- [ ] NotificationType 枚举（2个类型）
- [ ] NotificationStatus 枚举（3个状态）
- [ ] LoginStatus 枚举（6个状态）
- [ ] OperationType 枚举（11种操作）
- [ ] TargetType 枚举（5种类型）
- [ ] ChangeFieldType 枚举（6种类型）

### 9.2 业务实体实现
- [ ] ApplicationUser - 扩展现有实体
- [ ] Requirement - 核心实体
- [ ] Project - 项目实体
- [ ] Robot - 机器人实体
- [ ] NotificationLog - 通知日志
- [ ] VerificationCode - 短信验证码

### 9.3 审计实体实现
- [ ] LoginLog - 登录日志
- [ ] AuditSession - 审计会话
- [ ] OperationLog - 操作日志/敏感操作审计
- [ ] DataChangeLog - 数据变更日志/字段变更明细
- [ ] ApiAccessLog - API访问日志

### 9.4 关系配置
- [ ] Requirement.FollowerId → ApplicationUser.Id (Restrict)
- [ ] Requirement.ProjectId → Project.Id (Restrict)
- [ ] Requirement.RobotId → Robot.Id (SetNull)
- [ ] NotificationLog.RequirementId → Requirement.Id
- [ ] NotificationLog.RobotId → Robot.Id
- [ ] LoginLog.UserId → ApplicationUser.Id
- [ ] AuditSession.UserId → ApplicationUser.Id
- [ ] OperationLog.OperatorId → ApplicationUser.Id
- [ ] DataChangeLog.OperationLogId → OperationLog.Id (Cascade)
- [ ] ApiAccessLog.SessionId → AuditSession.Id
- [ ] ApiAccessLog.UserId → ApplicationUser.Id

### 9.5 业务规则实现
- [ ] 状态流转校验（相邻状态跳转）
- [ ] 终态不可变更（Online）
- [ ] 并发控制（乐观锁版本号）
- [ ] 删除约束检查
- [ ] 软删除过滤器
- [ ] 敏感操作审计记录
- [ ] API访问日志记录

---

## 10. 关键设计决策

### 10.1 主键类型选择
| 实体 | 主键类型 | 原因 |
|------|----------|------|
| ApplicationUser | Guid | 与 JWT ClaimTypes.NameIdentifier 一致 |
| 其他实体 | int (自增) | 性能更好，占用空间小 |

### 10.2 软删除策略
- 所有业务实体实现 `ICanSoftDelete` 接口
- EF Core 查询过滤器自动过滤软删除记录
- NotificationLog 和 LoginLog 不实现软删除（需保留历史记录）

### 10.3 审计字段策略
- 使用 `IHasCreatedOn`, `IHasCreator`, `IHasUpdater` 接口
- 时间使用 UTC
- Creator/Updater 存储 UserId

### 10.4 索引策略
- 高频查询字段建立索引
- 唯一约束建立唯一索引
- 复合索引优化筛选查询

---

**文档结束**
