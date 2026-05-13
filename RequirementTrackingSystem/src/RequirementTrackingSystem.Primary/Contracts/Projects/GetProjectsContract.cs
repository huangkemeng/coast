using RequirementTrackingSystem.Primary.Bases;
using RequirementTrackingSystem.Primary.Contracts.Bases;
using RequirementTrackingSystem.Infrastructure.DataPersistence.EfCore.Entities;
using Mediator.Net.Contracts;

namespace RequirementTrackingSystem.Primary.Contracts.Projects;

public class GetProjectsRequest : IRequest
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

public class ProjectListItem : IMapFrom<Project>
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string? ManagerName { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class GetProjectsResponse : IResponse, IPaginated
{
    public List<ProjectListItem> Items { get; set; } = new();
    public int PageIndex { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    public int Total { get; set; }
}

public interface IGetProjectsContract : IRequestContract<GetProjectsRequest, GetProjectsResponse>
{
}

public class GetProjectByIdQuery : IRequest
{
    public int Id { get; set; }
}

public class ProjectDetailResponse : IResponse, IMapFrom<Project>
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public int ManagerId { get; set; }
    public string? ManagerName { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
}

public interface IGetProjectByIdContract : IRequestContract<GetProjectByIdQuery, ProjectDetailResponse>
{
}