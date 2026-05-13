using RequirementTrackingSystem.Primary.Bases;
using RequirementTrackingSystem.Primary.Contracts.Bases;
using RequirementTrackingSystem.Infrastructure.DataPersistence.EfCore.Entities;
using Mediator.Net.Contracts;

namespace RequirementTrackingSystem.Primary.Contracts.Requirements;

public class GetRequirementsRequest : IRequest
{
    public string? Status { get; set; }
    public int? FollowerId { get; set; }
    public int? ProjectId { get; set; }
    public DateTime? PlanStartDateFrom { get; set; }
    public DateTime? PlanStartDateTo { get; set; }
    public DateTime? PlanTestDateFrom { get; set; }
    public DateTime? PlanTestDateTo { get; set; }
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortBy { get; set; }
    public string? SortOrder { get; set; }
}

public class RequirementListItem : IMapFrom<Requirement>
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string RequirementNo { get; set; } = string.Empty;
    public RequirementStatus Status { get; set; }
    public int Progress { get; set; }
    public string? FollowerName { get; set; }
    public string? ProjectName { get; set; }
    public Priority Priority { get; set; }
    public DateTime? PlanTestDate { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class GetRequirementsResponse : IResponse, IPaginated
{
    public List<RequirementListItem> Items { get; set; } = new();
    public int PageIndex { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    public int Total { get; set; }
}

public interface IGetRequirementsContract : IRequestContract<GetRequirementsRequest, GetRequirementsResponse>
{
}