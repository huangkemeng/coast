namespace RequirementTrackingSystem.Infrastructure.DataPersistence.EfCore.Entities;

public enum RequirementStatus
{
    PendingConfirm = 0,
    Confirmed = 1,
    PendingQuote = 2,
    Quoted = 3,
    PendingDev = 4,
    InDev = 5,
    InTest = 6,
    AcceptedPendingLaunch = 7,
    Launched = 8
}

public enum Priority
{
    Low = 0,
    Medium = 1,
    High = 2
}

public enum UserRole
{
    Admin = 0,
    Developer = 1,
    Tester = 2
}

public enum NotificationType
{
    StatusChange = 0,
    Reminder = 1,
    Test = 2
}

public enum NotificationStatus
{
    Pending = 0,
    Success = 1,
    Failed = 2
}