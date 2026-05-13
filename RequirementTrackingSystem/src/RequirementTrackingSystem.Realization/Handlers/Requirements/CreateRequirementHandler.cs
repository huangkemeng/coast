using RequirementTrackingSystem.Primary.Contracts.Requirements;
using RequirementTrackingSystem.Primary.Contracts.Bases;
using RequirementTrackingSystem.Realization.Bases;
using FluentValidation;
using Mediator.Net.Context;
using RequirementTrackingSystem.Infrastructure.DataPersistence.EfCore;
using RequirementTrackingSystem.Infrastructure.DataPersistence.EfCore.Entities;

namespace RequirementTrackingSystem.Realization.Handlers.Requirements;

public class CreateRequirementHandler : ICreateRequirementContract
{
    private readonly ApplicationDbContext _dbContext;

    public CreateRequirementHandler(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void Validate(ContractValidator<CreateRequirementCommand> validator)
    {
        validator.RuleFor(e => e.Name).NotEmpty().MaximumLength(100);
        validator.RuleFor(e => e.RequirementNo).NotEmpty().MaximumLength(50);
        validator.RuleFor(e => e.ProjectId).GreaterThan(0);
        validator.RuleFor(e => e.FollowerId).GreaterThan(0);
    }

    public async Task<CreateRequirementResponse> Handle(IReceiveContext<CreateRequirementCommand> context, CancellationToken cancellationToken)
    {
        var command = context.Message;
        var requirement = new Requirement
        {
            Name = command.Name,
            RequirementNo = command.RequirementNo,
            Status = RequirementStatus.PendingConfirm,
            Progress = 0,
            FollowerId = command.FollowerId,
            ProjectId = command.ProjectId,
            Priority = command.Priority,
            PlanStartDate = command.PlanStartDate,
            PlanTestDate = command.PlanTestDate,
            PlanLaunchDate = command.PlanLaunchDate,
            DocUrl = command.DocUrl,
            Price = command.Price,
            Remark = command.Remark,
            RobotId = command.RobotId,
            IsConfirmed = false,
            Version = 1,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _dbContext.Requirements.AddAsync(requirement, cancellationToken);
        return new CreateRequirementResponse { Id = requirement.Id };
    }

    public void Test(TestContext<CreateRequirementCommand, CreateRequirementResponse> context)
    {
        context.NoDatabase = true;
    }
}