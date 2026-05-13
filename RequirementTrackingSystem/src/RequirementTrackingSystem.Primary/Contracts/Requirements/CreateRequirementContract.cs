using RequirementTrackingSystem.Primary.Bases;
using RequirementTrackingSystem.Primary.Contracts.Bases;
using RequirementTrackingSystem.Infrastructure.DataPersistence.EfCore.Entities;
using Mediator.Net.Contracts;

namespace RequirementTrackingSystem.Primary.Contracts.Requirements;

public class CreateRequirementCommand : ICommand, IMapFrom<Requirement>
{
    public string Name { get; set; } = string.Empty;
    public string RequirementNo { get; set; } = string.Empty;
    public int ProjectId { get; set; }
    public int FollowerId { get; set; }
    public Priority Priority { get; set; }
    public DateTime? PlanStartDate { get; set; }
    public DateTime? PlanTestDate { get; set; }
    public DateTime? PlanLaunchDate { get; set; }
    public string? DocUrl { get; set; }
    public decimal? Price { get; set; }
    public string? Remark { get; set; }
    public int? RobotId { get; set; }
}

public class CreateRequirementResponse : IResponse
{
    public int Id { get; set; }
}

public interface ICreateRequirementContract : ICommandContract<CreateRequirementCommand, CreateRequirementResponse>
{
}