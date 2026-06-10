namespace Coast.Api.Infrastructure.DataPersistence.EfCore.Entities.Enums;

/// <summary>
/// 用户角色枚举
/// </summary>
public enum UserRole
{
    /// <summary>管理员（全部权限）</summary>
    Admin = 0,

    /// <summary>开发人员</summary>
    Developer = 1,

    /// <summary>测试人员</summary>
    Tester = 2
}
