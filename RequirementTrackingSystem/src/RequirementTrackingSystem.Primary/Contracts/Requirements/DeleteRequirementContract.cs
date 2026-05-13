using RequirementTrackingSystem.Primary.Contracts.Bases;
using RequirementTrackingSystem.Infrastructure.DataPersistence.EfCore.Entities;
using Mediator.Net.Contracts;

namespace RequirementTrackingSystem.Primary.Contracts.Requirements;

public class DeleteRequirementCommand : ICommand
{
    public int Id { get; set; }
}

public interface IDeleteRequirementContract : ICommandContract<DeleteRequirementCommand>
{
}