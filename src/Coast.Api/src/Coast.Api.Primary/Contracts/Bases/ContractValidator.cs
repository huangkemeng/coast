using FluentValidation;
using Mediator.Net.Contracts;

namespace Coast.Api.Primary.Contracts.Bases;

public class ContractValidator<TMessage> : AbstractValidator<TMessage> where TMessage : IMessage
{
}