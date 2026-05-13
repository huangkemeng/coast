using RequirementTrackingSystem.Primary.Contracts.Bases;
using Mediator.Net.Contracts;

namespace RequirementTrackingSystem.Primary.Contracts.Users;

public class DeleteUserCommand : ICommand
{
    public int Id { get; set; }
}

public interface IDeleteUserContract : ICommandContract<DeleteUserCommand>
{
}