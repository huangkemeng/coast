using RequirementTrackingSystem.Primary.Bases;
using RequirementTrackingSystem.Primary.Contracts.Bases;
using RequirementTrackingSystem.Infrastructure.DataPersistence.EfCore.Entities;
using Mediator.Net.Contracts;

namespace RequirementTrackingSystem.Primary.Contracts.Users;

public class CreateUserCommand : ICommand, IMapFrom<User>
{
    public string Username { get; set; } = string.Empty;
    public string RealName { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public bool IsEnabled { get; set; } = true;
}

public class CreateUserResponse : IResponse
{
    public int Id { get; set; }
}

public interface ICreateUserContract : ICommandContract<CreateUserCommand, CreateUserResponse>
{
}