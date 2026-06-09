using Autofac;
using Coast.Api.Primary.Contracts.Bases;
using Mediator.Net.Context;

namespace Coast.Api.Realization.Bases;

public class GetCurrentLifetimeScopeHandler(ILifetimeScope lifetimeScope) : IGetCurrentLifetimeScopeContract
{
    public Task<GetCurrentLifetimeScopeResponse> Handle(IReceiveContext<GetCurrentLifetimeScopeRequest> context,
        CancellationToken cancellationToken)
    {
        return Task.FromResult(new GetCurrentLifetimeScopeResponse
        {
            LifetimeScope = lifetimeScope
        });
    }

    public void Validate(ContractValidator<GetCurrentLifetimeScopeRequest> validator)
    {
    }

    public void Test(TestContext<GetCurrentLifetimeScopeRequest, GetCurrentLifetimeScopeResponse> context)
    {
    }
}