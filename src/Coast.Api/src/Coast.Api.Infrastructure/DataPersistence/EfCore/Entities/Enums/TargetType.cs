namespace Coast.Api.Infrastructure.DataPersistence.EfCore.Entities.Enums;

/// <summary>
/// 操作对象类型枚举
/// </summary>
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
