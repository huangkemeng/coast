using Mediator.Net.Contracts;

namespace Coast.Api.Primary.Contracts.Bases;

public interface ICommandContract<TCommand, TResponse> : IContract<TCommand>, ITestable<TCommand, TResponse>,
    ICommandHandler<TCommand, TResponse> where TCommand : ICommand where TResponse : IResponse
{
}

public interface ICommandContract<TCommand> : IContract<TCommand>, ITestable<TCommand>, ICommandHandler<TCommand>
    where TCommand : ICommand
{
}