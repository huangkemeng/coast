namespace Coast.Api.Infrastructure.DataPersistence.EfCore.Entities.Enums;

/// <summary>
/// 登录状态枚举
/// </summary>
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
