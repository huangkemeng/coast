using RequirementTrackingSystem.Primary.Contracts.Robots;
using RequirementTrackingSystem.Primary.Contracts.Bases;
using RequirementTrackingSystem.Realization.Bases;
using FluentValidation;
using Mediator.Net.Context;
using RequirementTrackingSystem.Infrastructure.DataPersistence.EfCore;
using RequirementTrackingSystem.Infrastructure.DataPersistence.EfCore.Entities;

namespace RequirementTrackingSystem.Realization.Handlers.Robots;

public class CreateRobotHandler : ICreateRobotContract
{
    private readonly ApplicationDbContext _dbContext;

    public CreateRobotHandler(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void Validate(ContractValidator<CreateRobotCommand> validator)
    {
        validator.RuleFor(e => e.Name).NotEmpty().MaximumLength(100);
        validator.RuleFor(e => e.WebhookUrl).NotEmpty().MaximumLength(500);
    }

    public async Task<CreateRobotResponse> Handle(IReceiveContext<CreateRobotCommand> context, CancellationToken cancellationToken)
    {
        var command = context.Message;
        var robot = new Robot
        {
            Name = command.Name,
            WebhookUrl = command.WebhookUrl,
            GroupName = command.GroupName,
            IsEnabled = command.IsEnabled,
            CreatedAt = DateTime.UtcNow
        };

        await _dbContext.Robots.AddAsync(robot, cancellationToken);
        return new CreateRobotResponse { Id = robot.Id };
    }

    public void Test(TestContext<CreateRobotCommand, CreateRobotResponse> context)
    {
        context.NoDatabase = true;
    }
}