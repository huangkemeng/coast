using RequirementTrackingSystem.Controllers.Bases;
using Mediator.Net;
using Microsoft.AspNetCore.Mvc.Filters;

namespace RequirementTrackingSystem.FilterAndMiddlewares;

public class AutoResolveFilter(IMediator mediator) : IAsyncActionFilter
{
    public Task OnActionExecutionAsync(
        ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var controller = context.Controller;
        if (controller is IHasMediator mediatorController)
        {
            mediatorController.Mediator = mediator;
        }

        return next();
    }
}