namespace Coast.Api.Infrastructure.DataPersistence.EfCore.Entities.Enums;

/// <summary>
/// 通知发送状态枚举
/// </summary>
public enum NotificationStatus
{
    /// <summary>待发送</summary>
    Pending = 0,

    /// <summary>发送成功</summary>
    Success = 1,

    /// <summary>发送失败</summary>
    Failed = 2
}
