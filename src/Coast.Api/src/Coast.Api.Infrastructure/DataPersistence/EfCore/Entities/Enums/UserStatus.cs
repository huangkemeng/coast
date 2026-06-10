namespace Coast.Api.Infrastructure.DataPersistence.EfCore.Entities.Enums;

/// <summary>
/// 用户状态枚举
/// </summary>
public enum UserStatus
{
    /// <summary>待启用</summary>
    Pending = 0,

    /// <summary>已启用</summary>
    Enabled = 1,

    /// <summary>已禁用</summary>
    Disabled = 2
}
