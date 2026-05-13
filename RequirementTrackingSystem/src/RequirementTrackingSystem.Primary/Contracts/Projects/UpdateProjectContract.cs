using RequirementTrackingSystem.Primary.Bases;
using RequirementTrackingSystem.Primary.Contracts.Bases;
using RequirementTrackingSystem.Infrastructure.DataPersistence.EfCore.Entities;
using Mediator.Net.Contracts;

namespace RequirementTrackingSystem.Primary.Contracts.Projects;

public class UpdateProjectCommand : ICommand, IMapFrom<Project>
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int ManagerId { get; set; }
    public string? Description { get; set; }
}

public class UpdateProjectResponse : IResponse
{
    public int Id { get; set; }
}

public interface IUpdateProjectContract : ICommandContract<UpdateProjectCommand, UpdateProjectResponse>
{
}