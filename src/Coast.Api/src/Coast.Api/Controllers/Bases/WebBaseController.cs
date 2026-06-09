using Coast.Api.Engines.SwaggerEngines;
using Microsoft.AspNetCore.Mvc;

namespace Coast.Api.Controllers.Bases;

[Route("api/web/[controller]")]
[SwaggerApiGroup(SwaggerApiGroupNames.Web)]
public class WebBaseController : BaseController
{
}