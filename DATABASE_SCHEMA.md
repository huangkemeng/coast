# 数据库实体文档

> 本文档描述项目中的数据实体结构，供 AI Coding 参考现有模式创建新实体。

---

## 1. 实体概览

| 实体名 | 说明 | 所属层 |
|--------|------|--------|
| `ApplicationUser` | 应用用户 | Infrastructure |
| `Requirement` | 需求 | 🔜 待定义 |
| `Project` | 项目 | 🔜 待定义 |
| `Robot` | 企业微信机器人 | 🔜 待定义 |
| `NotificationLog` | 通知日志 | 🔜 待定义 |

---

## 2. 实体基类

### 2.1 IEntity 接口
所有实体都需要实现此接口：
```csharp
public interface IEntity
{
}
```

### 2.2 审计接口
```csharp
public interface IHasCreatedOn { DateTime CreatedOn { get; set; } }
public interface IHasCreator<TKey> { TKey CreatorId { get; set; } }
public interface IHasUpdater<TKey> { TKey UpdaterId { get; set; } }
public interface IHasUpdatedOn { DateTime UpdatedOn { get; set; } }
```

### 2.3 软删除接口
```csharp
public interface ICanSoftDelete { bool IsDeleted { get; set; } }
```

### 2.4 排序接口
```csharp
public interface ISortable { int SortOrder { get; set; } }
```

---

## 3. ApplicationUser 实体

### 3.1 实体定义
文件位置: `Coast.Api.Infrastructure/DataPersistence/EfCore/Entities/ApplicationUser.cs`

```csharp
public class ApplicationUser : IEfEntity<ApplicationUser>, IHasKey<Guid>
{
    public ApplicationUser()
    {
        this.InitPropertyValues();
    }

    public static void ConfigureEntityMapping(
        EntityTypeBuilder<ApplicationUser> builder,
        IRelationalTypeMappingSource mappingSource)
    {
        builder.AutoConfigure(mappingSource);
    }

    public Guid Id { get; set; }
    // TODO: 待添加更多字段...
}
```

### 3.2 实现接口

| 接口 | 说明 |
|------|------|
| `IEfEntity<ApplicationUser>` | EF Core 实体标记 |
| `IHasKey<Guid>` | 主键定义 |

---

## 4. 需求实体 (待定义 - Requirement)

基于 PRD，建议结构如下：

### 4.1 字段定义

| 字段名 | 类型 | 必填 | 说明 |
|--------|------|------|------|
| Id | int | 是 | 主键，自增 |
| Name | string(100) | 是 | 需求名称 |
| Code | string(50) | 是 | 需求号，唯一 |
| Status | enum | 是 | 状态（9种） |
| Progress | int | 否 | 进度 0-100 |
| FollowerId | Guid | 是 | 跟进人 |
| ProjectId | int | 是 | 所属项目 |
| RobotId | int | 否 | 通知机器人 |
| Priority | enum | 否 | 高/中/低 |
| PlanStartDate | DateTime | 否 | 计划开始时间 |
| PlanTestDate | DateTime | 否 | 计划交测时间 |
| PlanOnlineDate | DateTime | 否 | 计划上线时间 |
| ActualTestDate | DateTime | 否 | 实际交测时间 |
| ActualOnlineDate | DateTime | 否 | 实际上线时间 |
| IsConfirmed | bool | 否 | 需求已确认 |
| DocumentUrl | string | 否 | 需求文档链接 |
| Quote | decimal | 否 | 报价（仅管理员） |
| Remark | string(500) | 否 | 备注 |
| Version | int | 是 | 并发控制版本号 |
| CreatedOn | DateTime | 是 | 创建时间 |
| CreatorId | Guid | 是 | 创建人 |
| UpdatedOn | DateTime | 是 | 更新时间 |
| UpdaterId | Guid | 是 | 更新人 |

### 4.2 状态枚举

```csharp
public enum RequirementStatus
{
    Pending = 0,           // 待确认
    Confirmed = 1,         // 已确认
    PendingQuote = 2,      // 待报价
    Quoted = 3,            // 已报价
    PendingDev = 4,        // 待开发
    Developing = 5,        // 开发中
    Testing = 6,           // 测试中
    AcceptedPendingOnline = 7,  // 已验收待上线
    Online = 8             // 已上线（终态）
}
```

### 4.3 优先级枚举

```csharp
public enum RequirementPriority
{
    Low = 0,      // 低
    Medium = 1,   // 中（默认）
    High = 2      // 高
}
```

---

## 5. 项目实体 (待定义 - Project)

### 5.1 字段定义

| 字段名 | 类型 | 必填 | 说明 |
|--------|------|------|------|
| Id | int | 是 | 主键，自增 |
| Name | string(100) | 是 | 项目名称 |
| Code | string(50) | 否 | 项目编码 |
| ManagerId | Guid | 否 | 项目负责人 |
| Description | string(500) | 否 | 项目描述 |
| IsDeleted | bool | 是 | 软删除标记 |
| CreatedOn | DateTime | 是 | 创建时间 |
| UpdatedOn | DateTime | 是 | 更新时间 |

---

## 6. 机器人实体 (待定义 - Robot)

### 6.1 字段定义

| 字段名 | 类型 | 必填 | 说明 |
|--------|------|------|------|
| Id | int | 是 | 主键，自增 |
| Name | string(50) | 是 | 机器人名称 |
| WebhookUrl | string(500) | 是 | Webhook地址 |
| GroupName | string(100) | 否 | 所属群组 |
| IsEnabled | bool | 是 | 启用状态 |
| IsDeleted | bool | 是 | 软删除标记 |
| CreatedOn | DateTime | 是 | 创建时间 |
| UpdatedOn | DateTime | 是 | 更新时间 |

---

## 7. 通知日志实体 (待定义 - NotificationLog)

### 7.1 字段定义

| 字段名 | 类型 | 必填 | 说明 |
|--------|------|------|------|
| Id | int | 是 | 主键，自增 |
| RequirementId | int | 是 | 关联需求 |
| RobotId | int | 是 | 接收机器人 |
| Type | enum | 是 | 通知类型 |
| Status | enum | 是 | 发送状态 |
| Content | string | 是 | 通知内容 |
| ErrorMessage | string | 否 | 失败原因 |
| RetryCount | int | 是 | 重试次数 |
| SentAt | DateTime | 是 | 发送时间 |
| CreatedOn | DateTime | 是 | 创建时间 |

### 7.2 通知类型枚举

```csharp
public enum NotificationType
{
    StatusChange = 0,   // 状态变更
    TimeReminder = 1   // 时间提醒
}
```

### 7.3 发送状态枚举

```csharp
public enum NotificationStatus
{
    Pending = 0,     // 待发送
    Success = 1,     // 成功
    Failed = 2       // 失败
}
```

---

## 8. 实体创建模板

### 8.1 EF Core 实体模板

```csharp
using Coast.Api.Infrastructure.DataPersistence.DataEntityBases;
using Coast.Api.Infrastructure.DataPersistence.EfCore.Entities.Bases;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage;

namespace Coast.Api.Infrastructure.DataPersistence.EfCore.Entities;

/// <summary>
/// {实体中文名称}
/// </summary>
public class {EntityName} : IEfEntity<{EntityName}>, 
    IHasKey<int>,           // 或 Guid
    ICanSoftDelete,
    IHasCreatedOn,
    IHasUpdatedOn
{
    public {EntityName}()
    {
        this.InitPropertyValues();
    }

    public static void ConfigureEntityMapping(
        EntityTypeBuilder<{EntityName}> builder,
        IRelationalTypeMappingSource mappingSource)
    {
        builder.AutoConfigure(mappingSource);
        
        // 自定义配置
        builder.Property(x => x.Name).HasMaxLength(100).IsRequired();
        builder.HasIndex(x => x.Code).IsUnique();
    }

    // 主键
    public int Id { get; set; }
    
    // TODO: 添加其他字段...
    
    // 审计字段
    public bool IsDeleted { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime UpdatedOn { get; set; }
}
```

---

## 9. 实体关系图

```
┌─────────────────┐     ┌─────────────────┐
│   ApplicationUser │     │     Project     │
├─────────────────┤     ├─────────────────┤
│ Id (PK)         │     │ Id (PK)         │
│ Username        │     │ Name            │
│ Name            │     │ Code            │
│ ...             │     │ ManagerId (FK)  │───┐
└─────────────────┘     └─────────────────┘   │
    │                                           │
    │ 1:N                    1:N                  │
    ▼                     ▼                     │
┌─────────────────┐     ┌─────────────────┐   │
│   Requirement   │     │     Robot        │
├─────────────────┤     ├─────────────────┤   │
│ Id (PK)         │     │ Id (PK)         │◄──┘
│ Name            │     │ Name            │
│ Code            │     │ WebhookUrl      │
│ Status          │     │ IsEnabled       │
│ FollowerId (FK)─┼────►│                 │
│ ProjectId (FK)──┼────►│                 │
│ RobotId (FK)   ─┼────►│                 │
│ Quote           │     └─────────────────┘
│ Version         │
│ ...             │     ┌─────────────────┐
└─────────────────┘     │ NotificationLog │
                        ├─────────────────┤
                        │ Id (PK)         │
                        │ RequirementId   │
                        │ RobotId (FK)────┼────┐
                        │ Type            │     │
                        │ Status          │     │
                        │ ...             │     │
                        └─────────────────┘     │
                                                 │
                        需求跟踪系统核心实体关系 ◄─┘
```

---

**维护者**: AI Coding Team  
**版本**: v1.0  
**最后更新**: 2026-06-10