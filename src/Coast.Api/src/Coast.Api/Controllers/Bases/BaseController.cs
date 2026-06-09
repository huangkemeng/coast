using Coast.Api.FilterAndMiddlewares;
using Mediator.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Coast.Api.Controllers.Bases;

[ApiController]
// [Authorize]
[ServiceFilter<AutoResolveFilter>]
[TypeFilter(typeof(HandleTimezoneResultFilter))]
public class BaseController : ControllerBase, IHasMediator
{
    public IMediator Mediator { get; set; }
}