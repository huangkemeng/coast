using RequirementTrackingSystem.Primary.Bases;
using RequirementTrackingSystem.Primary.Contracts.Bases;
using RequirementTrackingSystem.Infrastructure.DataPersistence.EfCore.Entities;
using Mediator.Net.Contracts;

namespace RequirementTrackingSystem.Primary.Contracts.Robots;

public class CreateRobotCommand : ICommand, IMapFrom<Robot>
{
    public string Name { get; set; } = string.Empty;
    public string WebhookUrl { get; set; } = string.Empty;
    public string? GroupName { get; set; }
    public bool IsEnabled { get; set; } = true;
}

public class CreateRobotResponse : IResponse
{
    public int Id { get; set; }
}

public interface ICreateRobotContract : ICommandContract<CreateRobotCommand, CreateRobotResponse>
{
}