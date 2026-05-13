using RequirementTrackingSystem.Primary.Contracts.Bases;
using Mediator.Net.Contracts;

namespace RequirementTrackingSystem.Primary.Contracts.Projects;

public class DeleteProjectCommand : ICommand
{
    public int Id { get; set; }
}

public interface IDeleteProjectContract : ICommandContract<DeleteProjectCommand>
{
}