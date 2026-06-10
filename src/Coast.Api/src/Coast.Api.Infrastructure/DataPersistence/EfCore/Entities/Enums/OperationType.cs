namespace Coast.Api.Infrastructure.DataPersistence.EfCore.Entities.Enums;

/// <summary>
/// 操作类型枚举
/// </summary>
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
