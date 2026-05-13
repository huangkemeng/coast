using RequirementTrackingSystem.Primary.Bases;
using RequirementTrackingSystem.Primary.Contracts.Bases;
using RequirementTrackingSystem.Infrastructure.DataPersistence.EfCore.Entities;
using Mediator.Net.Contracts;

namespace RequirementTrackingSystem.Primary.Contracts.Users;

public class UpdateUserCommand : ICommand, IMapFrom<User>
{
    public int Id { get; set; }
    public string RealName { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public bool IsEnabled { get; set; }
}

public class UpdateUserResponse : IResponse
{
    public int Id { get; set; }
}

public interface IUpdateUserContract : ICommandContract<UpdateUserCommand, UpdateUserResponse>
{
}