using RequirementTrackingSystem.Primary.Contracts.Robots;
using RequirementTrackingSystem.Primary.Contracts.Bases;
using RequirementTrackingSystem.Realization.Bases;
using FluentValidation;
using Mediator.Net.Context;
using RequirementTrackingSystem.Infrastructure.DataPersistence.EfCore;
using RequirementTrackingSystem.Infrastructure.DataPersistence.EfCore.Entities;

namespace RequirementTrackingSystem.Realization.Handlers.Robots;

public class UpdateRobotHandler : IUpdateRobotContract
{
    private readonly ApplicationDbContext _dbContext;

    public UpdateRobotHandler(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void Validate(ContractValidator<UpdateRobotCommand> validator)
    {
        validator.RuleFor(e => e.Name).NotEmpty().MaximumLength(100);
        validator.RuleFor(e => e.WebhookUrl).NotEmpty().MaximumLength(500);
    }

    public async Task<UpdateRobotResponse> Handle(IReceiveContext<UpdateRobotCommand> context, CancellationToken cancellationToken)
    {
        var command = context.Message;
        var robot = await _dbContext.Robots.FindAsync(new object[] { command.Id }, cancellationToken);

        if (robot == null)
        {
            throw new BusinessException("机器人不存在", BusinessExceptionTypeEnum.NotSpecified, "ROBOT001");
        }

        robot.Name = command.Name;
        robot.WebhookUrl = command.WebhookUrl;
        robot.GroupName = command.GroupName;
        robot.IsEnabled = command.IsEnabled;

        return new UpdateRobotResponse { Id = robot.Id };
    }

    public void Test(TestContext<UpdateRobotCommand, UpdateRobotResponse> context)
    {
        context.NoDatabase = true;
    }
}