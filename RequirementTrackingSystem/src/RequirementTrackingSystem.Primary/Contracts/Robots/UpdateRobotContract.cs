using RequirementTrackingSystem.Primary.Bases;
using RequirementTrackingSystem.Primary.Contracts.Bases;
using RequirementTrackingSystem.Infrastructure.DataPersistence.EfCore.Entities;
using Mediator.Net.Contracts;

namespace RequirementTrackingSystem.Primary.Contracts.Robots;

public class UpdateRobotCommand : ICommand, IMapFrom<Robot>
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string WebhookUrl { get; set; } = string.Empty;
    public string? GroupName { get; set; }
    public bool IsEnabled { get; set; }
}

public class UpdateRobotResponse : IResponse
{
    public int Id { get; set; }
}

public interface IUpdateRobotContract : ICommandContract<UpdateRobotCommand, UpdateRobotResponse>
{
}