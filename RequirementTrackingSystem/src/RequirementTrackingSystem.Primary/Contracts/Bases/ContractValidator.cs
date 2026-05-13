using FluentValidation;
using Mediator.Net.Contracts;

namespace RequirementTrackingSystem.Primary.Contracts.Bases;

public class ContractValidator<TMessage> : AbstractValidator<TMessage> where TMessage : IMessage
{
}