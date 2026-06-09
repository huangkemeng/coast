using FluentValidation;
using Mediator.Net.Contracts;

namespace Coast.Api.Primary.Contracts.Bases;

public class ContractValidationContext : ValidationContext<IMessage>
{
    public ContractValidationContext(IMessage message) : base(message)
    {
    }
}