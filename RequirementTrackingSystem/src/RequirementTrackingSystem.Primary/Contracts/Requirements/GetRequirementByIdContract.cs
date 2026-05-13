using RequirementTrackingSystem.Primary.Bases;
using RequirementTrackingSystem.Primary.Contracts.Bases;
using RequirementTrackingSystem.Infrastructure.DataPersistence.EfCore.Entities;
using Mediator.Net.Contracts;

namespace RequirementTrackingSystem.Primary.Contracts.Requirements;

public class GetRequirementByIdQuery : IRequest, IMapFrom<Requirement>
{
    public int Id { get; set; }
}

public class RequirementDetailResponse : IResponse, IMapFrom<Requirement>
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string RequirementNo { get; set; } = string.Empty;
    public RequirementStatus Status { get; set; }
    public int Progress { get; set; }
    public int FollowerId { get; set; }
    public string? FollowerName { get; set; }
    public DateTime? PlanStartDate { get; set; }
    public DateTime? PlanTestDate { get; set; }
    public DateTime? PlanLaunchDate { get; set; }
    public DateTime? ActualTestDate { get; set; }
    public DateTime? ActualLaunchDate { get; set; }
    public bool IsConfirmed { get; set; }
    public string? DocUrl { get; set; }
    public decimal? Price { get; set; }
    public int ProjectId { get; set; }
    public string? ProjectName { get; set; }
    public int? RobotId { get; set; }
    public string? RobotName { get; set; }
    public Priority Priority { get; set; }
    public string? Remark { get; set; }
    public int Version { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public interface IGetRequirementByIdContract : IRequestContract<GetRequirementByIdQuery, RequirementDetailResponse>
{
}