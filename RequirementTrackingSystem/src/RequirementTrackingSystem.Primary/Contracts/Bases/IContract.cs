using Mediator.Net.Contracts;

namespace RequirementTrackingSystem.Primary.Contracts.Bases;

public interface IContract<T> where T : IMessage
{
    void Validate(ContractValidator<T> validator);
}