using RequirementTrackingSystem.Primary.Contracts.Requirements;
using RequirementTrackingSystem.Primary.Contracts.Bases;
using RequirementTrackingSystem.Realization.Bases;
using FluentValidation;
using Mediator.Net.Context;
using RequirementTrackingSystem.Infrastructure.DataPersistence.EfCore;
using RequirementTrackingSystem.Infrastructure.DataPersistence.EfCore.Entities;

namespace RequirementTrackingSystem.Realization.Handlers.Requirements;

public class UpdateRequirementHandler : IUpdateRequirementContract
{
    private readonly ApplicationDbContext _dbContext;

    public UpdateRequirementHandler(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void Validate(ContractValidator<UpdateRequirementCommand> validator)
    {
        validator.RuleFor(e => e.Name).NotEmpty().MaximumLength(100);
        validator.RuleFor(e => e.ProjectId).GreaterThan(0);
        validator.RuleFor(e => e.FollowerId).GreaterThan(0);
        validator.RuleFor(e => e.Version).GreaterThan(0);
    }

    public async Task<UpdateRequirementResponse> Handle(IReceiveContext<UpdateRequirementCommand> context, CancellationToken cancellationToken)
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

        requirement.Name = command.Name;
        requirement.ProjectId = command.ProjectId;
        requirement.FollowerId = command.FollowerId;
        requirement.Priority = command.Priority;
        requirement.PlanStartDate = command.PlanStartDate;
        requirement.PlanTestDate = command.PlanTestDate;
        requirement.PlanLaunchDate = command.PlanLaunchDate;
        requirement.DocUrl = command.DocUrl;
        requirement.Price = command.Price;
        requirement.Remark = command.Remark;
        requirement.RobotId = command.RobotId;
        requirement.Version++;
        requirement.UpdatedAt = DateTime.UtcNow;

        return new UpdateRequirementResponse { Id = requirement.Id };
    }

    public void Test(TestContext<UpdateRequirementCommand, UpdateRequirementResponse> context)
    {
        context.NoDatabase = true;
    }
}