using RequirementTrackingSystem.Primary.Contracts.Robots;
using RequirementTrackingSystem.Primary.Contracts.Bases;
using RequirementTrackingSystem.Realization.Bases;
using FluentValidation;
using Mediator.Net.Context;
using RequirementTrackingSystem.Infrastructure.DataPersistence.EfCore;

namespace RequirementTrackingSystem.Realization.Handlers.Robots;

public class DeleteRobotHandler : IDeleteRobotContract
{
    private readonly ApplicationDbContext _dbContext;

    public DeleteRobotHandler(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void Validate(ContractValidator<DeleteRobotCommand> validator)
    {
        validator.RuleFor(e => e.Id).GreaterThan(0);
    }

    public async Task Handle(IReceiveContext<DeleteRobotCommand> context, CancellationToken cancellationToken)
    {
        var command = context.Message;
        var robot = await _dbContext.Robots.FindAsync(new object[] { command.Id }, cancellationToken);

        if (robot == null)
        {
            throw new BusinessException("机器人不存在", BusinessExceptionTypeEnum.NotSpecified, "ROBOT001");
        }

        _dbContext.Robots.Remove(robot);
    }

    public void Test(TestContext<DeleteRobotCommand> context)
    {
        context.NoDatabase = true;
    }
}