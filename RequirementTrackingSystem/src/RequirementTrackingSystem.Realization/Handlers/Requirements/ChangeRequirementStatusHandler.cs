using RequirementTrackingSystem.Primary.Contracts.Requirements;
using RequirementTrackingSystem.Primary.Contracts.Bases;
using RequirementTrackingSystem.Realization.Bases;
using FluentValidation;
using Mediator.Net.Context;
using RequirementTrackingSystem.Infrastructure.DataPersistence.EfCore;
using RequirementTrackingSystem.Infrastructure.DataPersistence.EfCore.Entities;

namespace RequirementTrackingSystem.Realization.Handlers.Requirements;

public class ChangeRequirementStatusHandler : IChangeRequirementStatusContract
{
    private static readonly Dictionary<RequirementStatus, RequirementStatus[]> ValidTransitions = new()
    {
        { RequirementStatus.PendingConfirm, new[] { RequirementStatus.Confirmed } },
        { RequirementStatus.Confirmed, new[] { RequirementStatus.PendingQuote } },
        { RequirementStatus.PendingQuote, new[] { RequirementStatus.Quoted } },
        { RequirementStatus.Quoted, new[] { RequirementStatus.PendingDev } },
        { RequirementStatus.PendingDev, new[] { RequirementStatus.InDev } },
        { RequirementStatus.InDev, new[] { RequirementStatus.InTest } },
        { RequirementStatus.InTest, new[] { RequirementStatus.AcceptedPendingLaunch } },
        { RequirementStatus.AcceptedPendingLaunch, new[] { RequirementStatus.Launched } },
        { RequirementStatus.Launched, Array.Empty<RequirementStatus>() }
    };

    private readonly ApplicationDbContext _dbContext;

    public ChangeRequirementStatusHandler(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void Validate(ContractValidator<ChangeRequirementStatusCommand> validator)
    {
        validator.RuleFor(e => e.Id).GreaterThan(0);
        validator.RuleFor(e => e.Version).GreaterThan(0);
    }

    public async Task<ChangeRequirementStatusResponse> Handle(IReceiveContext<ChangeRequirementStatusCommand> context, CancellationToken cancellationToken)
    {
        var command = context.Message;
        var requirement = await _dbContext.Requirements.FindAsync(new object[] { command.Id }, cancellationToken);

        if (requirement == null)
        {
            throw new BusinessException("需求不存在", BusinessExceptionTypeEnum.NotSpecified, "REQ001");
        }

        if (requirement.Version != command.Version)
        {
            throw new BusinessException("数据已被他人修改，请刷新页面获取最新数据后重新编辑", BusinessExceptionTypeEnum.NotSpecified, "REQ002");
        }

        if (!ValidTransitions.TryGetValue(requirement.Status, out var validTargets) || !validTargets.Contains(command.NewStatus))
        {
            throw new BusinessException($"不允许从状态 {requirement.Status} 直接变更为 {command.NewStatus}", BusinessExceptionTypeEnum.NotSpecified, "REQ003");
        }

        requirement.Status = command.NewStatus;
        requirement.Version++;
        requirement.UpdatedAt = DateTime.UtcNow;

        if (command.NewStatus >= RequirementStatus.Confirmed)
        {
            requirement.IsConfirmed = true;
        }

        if (command.NewStatus == RequirementStatus.InTest)
        {
            requirement.ActualTestDate = DateTime.UtcNow;
        }

        if (command.NewStatus == RequirementStatus.Launched)
        {
            requirement.ActualLaunchDate = DateTime.UtcNow;
            requirement.Progress = 100;
        }

        return new ChangeRequirementStatusResponse { Id = requirement.Id, Status = requirement.Status };
    }

    public void Test(TestContext<ChangeRequirementStatusCommand, ChangeRequirementStatusResponse> context)
    {
        context.NoDatabase = true;
    }
}