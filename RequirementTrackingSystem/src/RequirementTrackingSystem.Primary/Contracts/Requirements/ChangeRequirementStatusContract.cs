using RequirementTrackingSystem.Primary.Contracts.Bases;
using RequirementTrackingSystem.Infrastructure.DataPersistence.EfCore.Entities;
using Mediator.Net.Contracts;

namespace RequirementTrackingSystem.Primary.Contracts.Requirements;

public class ChangeRequirementStatusCommand : ICommand
{
    public int Id { get; set; }
    public RequirementStatus NewStatus { get; set; }
    public int Version { get; set; }
}

public class ChangeRequirementStatusResponse : IResponse
{
    public int Id { get; set; }
    public RequirementStatus Status { get; set; }
}

public interface IChangeRequirementStatusContract : ICommandContract<ChangeRequirementStatusCommand, ChangeRequirementStatusResponse>
{
}