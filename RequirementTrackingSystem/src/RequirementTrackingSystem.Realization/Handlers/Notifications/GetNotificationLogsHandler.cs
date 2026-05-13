using RequirementTrackingSystem.Primary.Contracts.Notifications;
using RequirementTrackingSystem.Primary.Contracts.Bases;
using RequirementTrackingSystem.Realization.Bases;
using FluentValidation;
using Mediator.Net.Context;
using RequirementTrackingSystem.Infrastructure.DataPersistence.EfCore;
using RequirementTrackingSystem.Infrastructure.DataPersistence.EfCore.Entities;
using Microsoft.EntityFrameworkCore;

namespace RequirementTrackingSystem.Realization.Handlers.Notifications;

public class GetNotificationLogsHandler : IGetNotificationLogsContract
{
    private readonly ApplicationDbContext _dbContext;

    public GetNotificationLogsHandler(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void Validate(ContractValidator<GetNotificationLogsRequest> validator)
    {
        validator.RuleFor(e => e.PageIndex).GreaterThanOrEqualTo(1);
        validator.RuleFor(e => e.PageSize).InclusiveBetween(1, 100);
    }

    public async Task<GetNotificationLogsResponse> Handle(IReceiveContext<GetNotificationLogsRequest> context, CancellationToken cancellationToken)
    {
        var query = context.Message;
        var queryable = _dbContext.NotificationLogs
            .Include(n => n.Requirement)
            .Include(n => n.Robot)
            .AsQueryable();

        if (query.RequirementId.HasValue)
        {
            queryable = queryable.Where(n => n.RequirementId == query.RequirementId.Value);
        }

        if (query.Type.HasValue)
        {
            queryable = queryable.Where(n => n.Type == query.Type.Value);
        }

        if (query.Status.HasValue)
        {
            queryable = queryable.Where(n => n.Status == query.Status.Value);
        }

        if (query.FromDate.HasValue)
        {
            queryable = queryable.Where(n => n.SentAt >= query.FromDate.Value);
        }

        if (query.ToDate.HasValue)
        {
            queryable = queryable.Where(n => n.SentAt <= query.ToDate.Value);
        }

        var totalCount = await queryable.CountAsync(cancellationToken);
        var items = await queryable
            .OrderByDescending(n => n.SentAt)
            .Skip((query.PageIndex - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(n => new NotificationLogItem
            {
                Id = n.Id,
                RequirementId = n.RequirementId,
                RequirementName = n.Requirement != null ? n.Requirement.Name : null,
                Type = n.Type,
                RobotId = n.RobotId,
                RobotName = n.Robot != null ? n.Robot.Name : null,
                Status = n.Status,
                SentAt = n.SentAt,
                ErrorMessage = n.ErrorMessage,
                RetryCount = n.RetryCount,
                LastAttemptAt = n.LastAttemptAt
            })
            .ToListAsync(cancellationToken);

        return new GetNotificationLogsResponse
        {
            Items = items,
            PageIndex = query.PageIndex,
            PageSize = query.PageSize,
            TotalCount = totalCount,
            Total = totalCount
        };
    }

    public void Test(TestContext<GetNotificationLogsRequest, GetNotificationLogsResponse> context)
    {
    }
}

public class GetNotificationLogByIdHandler : IGetNotificationLogByIdContract
{
    private readonly ApplicationDbContext _dbContext;

    public GetNotificationLogByIdHandler(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void Validate(ContractValidator<GetNotificationLogByIdQuery> validator)
    {
        validator.RuleFor(e => e.Id).GreaterThan(0);
    }

    public async Task<NotificationLogDetailResponse> Handle(IReceiveContext<GetNotificationLogByIdQuery> context, CancellationToken cancellationToken)
    {
        var query = context.Message;
        var notificationLog = await _dbContext.NotificationLogs
            .Include(n => n.Requirement)
            .Include(n => n.Robot)
            .FirstOrDefaultAsync(n => n.Id == query.Id, cancellationToken);

        if (notificationLog == null)
        {
            throw new BusinessException("通知日志不存在", BusinessExceptionTypeEnum.NotSpecified, "NOTIFY001");
        }

        return new NotificationLogDetailResponse
        {
            Id = notificationLog.Id,
            RequirementId = notificationLog.RequirementId,
            RequirementName = notificationLog.Requirement?.Name,
            Type = notificationLog.Type,
            RobotId = notificationLog.RobotId,
            RobotName = notificationLog.Robot?.Name,
            Status = notificationLog.Status,
            SentAt = notificationLog.SentAt,
            ErrorMessage = notificationLog.ErrorMessage,
            RetryCount = notificationLog.RetryCount,
            LastAttemptAt = notificationLog.LastAttemptAt
        };
    }

    public void Test(TestContext<GetNotificationLogByIdQuery, NotificationLogDetailResponse> context)
    {
    }
}