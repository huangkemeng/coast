using Coast.Api.Engines.SwaggerEngines;
using Microsoft.AspNetCore.Mvc;

namespace Coast.Api.Controllers.Bases;

[Route("api/app/[controller]")]
[SwaggerApiGroup(SwaggerApiGroupNames.App)]
public class AppBaseController : BaseController
{
}