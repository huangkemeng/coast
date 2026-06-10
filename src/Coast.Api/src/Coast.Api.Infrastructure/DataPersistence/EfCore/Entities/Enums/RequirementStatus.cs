namespace Coast.Api.Infrastructure.DataPersistence.EfCore.Entities.Enums;

/// <summary>
/// 需求状态枚举（9个状态，线性流转）
/// </summary>
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
