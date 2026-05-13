using RequirementTrackingSystem.Primary.Contracts.Requirements;
using RequirementTrackingSystem.Primary.Contracts.Bases;
using RequirementTrackingSystem.Realization.Bases;
using FluentValidation;
using Mediator.Net.Context;
using RequirementTrackingSystem.Infrastructure.DataPersistence.EfCore;
using Microsoft.EntityFrameworkCore;

namespace RequirementTrackingSystem.Realization.Handlers.Requirements;

public class GetRequirementByIdHandler : IGetRequirementByIdContract
{
    private readonly ApplicationDbContext _dbContext;

    public GetRequirementByIdHandler(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void Validate(ContractValidator<GetRequirementByIdQuery> validator)
    {
        validator.RuleFor(e => e.Id).GreaterThan(0);
    }

    public async Task<RequirementDetailResponse> Handle(IReceiveContext<GetRequirementByIdQuery> context, CancellationToken cancellationToken)
    {
        var query = context.Message;
        var requirement = await _dbContext.Requirements
            .Include(r => r.Follower)
            .Include(r => r.Project)
            .Include(r => r.Robot)
            .FirstOrDefaultAsync(r => r.Id == query.Id, cancellationToken);

        if (requirement == null)
        {
            throw new BusinessException("需求不存在", BusinessExceptionTypeEnum.NotSpecified, "REQ001");
        }

        return new RequirementDetailResponse
        {
            Id = requirement.Id,
            Name = requirement.Name,
            RequirementNo = requirement.RequirementNo,
            Status = requirement.Status,
            Progress = requirement.Progress,
            FollowerId = requirement.FollowerId,
            FollowerName = requirement.Follower?.RealName,
            PlanStartDate = requirement.PlanStartDate,
            PlanTestDate = requirement.PlanTestDate,
            PlanLaunchDate = requirement.PlanLaunchDate,
            ActualTestDate = requirement.ActualTestDate,
            ActualLaunchDate = requirement.ActualLaunchDate,
            IsConfirmed = requirement.IsConfirmed,
            DocUrl = requirement.DocUrl,
            Price = requirement.Price,
            ProjectId = requirement.ProjectId,
            ProjectName = requirement.Project?.Name,
            RobotId = requirement.RobotId,
            RobotName = requirement.Robot?.Name,
            Priority = requirement.Priority,
            Remark = requirement.Remark,
            Version = requirement.Version,
            CreatedAt = requirement.CreatedAt,
            UpdatedAt = requirement.UpdatedAt
        };
    }

    public void Test(TestContext<GetRequirementByIdQuery, RequirementDetailResponse> context)
    {
    }
}