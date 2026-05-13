using RequirementTrackingSystem.Primary.Bases;
using RequirementTrackingSystem.Primary.Contracts.Bases;
using RequirementTrackingSystem.Infrastructure.DataPersistence.EfCore.Entities;
using Mediator.Net.Contracts;

namespace RequirementTrackingSystem.Primary.Contracts.Projects;

public class CreateProjectCommand : ICommand, IMapFrom<Project>
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public int ManagerId { get; set; }
    public string? Description { get; set; }
}

public class CreateProjectResponse : IResponse
{
    public int Id { get; set; }
}

public interface ICreateProjectContract : ICommandContract<CreateProjectCommand, CreateProjectResponse>
{
}