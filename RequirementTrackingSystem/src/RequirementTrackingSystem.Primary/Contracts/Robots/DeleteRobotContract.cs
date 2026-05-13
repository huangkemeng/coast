using RequirementTrackingSystem.Primary.Contracts.Bases;
using Mediator.Net.Contracts;

namespace RequirementTrackingSystem.Primary.Contracts.Robots;

public class DeleteRobotCommand : ICommand
{
    public int Id { get; set; }
}

public interface IDeleteRobotContract : ICommandContract<DeleteRobotCommand>
{
}