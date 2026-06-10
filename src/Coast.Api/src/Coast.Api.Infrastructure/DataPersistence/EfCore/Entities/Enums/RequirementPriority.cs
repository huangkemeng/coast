namespace Coast.Api.Infrastructure.DataPersistence.EfCore.Entities.Enums;

/// <summary>
/// 需求优先级枚举
/// </summary>
public enum RequirementPriority
{
    /// <summary>低优先级</summary>
    Low = 0,

    /// <summary>中优先级（默认）</summary>
    Medium = 1,

    /// <summary>高优先级</summary>
    High = 2
}
