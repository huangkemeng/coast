using RequirementTrackingSystem.Primary.Bases;
using RequirementTrackingSystem.Primary.Contracts.Bases;
using RequirementTrackingSystem.Infrastructure.DataPersistence.EfCore.Entities;
using Mediator.Net.Contracts;

namespace RequirementTrackingSystem.Primary.Contracts.Notifications;

public class GetNotificationLogsRequest : IRequest
{
    public int? RequirementId { get; set; }
    public NotificationType? Type { get; set; }
    public NotificationStatus? Status { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

public class NotificationLogItem : IMapFrom<NotificationLog>
{
    public int Id { get; set; }
    public int RequirementId { get; set; }
    public string? RequirementName { get; set; }
    public NotificationType Type { get; set; }
    public int RobotId { get; set; }
    public string? RobotName { get; set; }
    public NotificationStatus Status { get; set; }
    public DateTime? SentAt { get; set; }
    public string? ErrorMessage { get; set; }
    public int RetryCount { get; set; }
    public DateTime? LastAttemptAt { get; set; }
}

public class GetNotificationLogsResponse : IResponse, IPaginated
{
    public List<NotificationLogItem> Items { get; set; } = new();
    public int PageIndex { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    public int Total { get; set; }
}

public interface IGetNotificationLogsContract : IRequestContract<GetNotificationLogsRequest, GetNotificationLogsResponse>
{
}

public class GetNotificationLogByIdQuery : IRequest
{
    public int Id { get; set; }
}

public class NotificationLogDetailResponse : IResponse, IMapFrom<NotificationLog>
{
    public int Id { get; set; }
    public int RequirementId { get; set; }
    public string? RequirementName { get; set; }
    public NotificationType Type { get; set; }
    public int RobotId { get; set; }
    public string? RobotName { get; set; }
    public NotificationStatus Status { get; set; }
    public DateTime? SentAt { get; set; }
    public string? ErrorMessage { get; set; }
    public int RetryCount { get; set; }
    public DateTime? LastAttemptAt { get; set; }
}

public interface IGetNotificationLogByIdContract : IRequestContract<GetNotificationLogByIdQuery, NotificationLogDetailResponse>
{
}