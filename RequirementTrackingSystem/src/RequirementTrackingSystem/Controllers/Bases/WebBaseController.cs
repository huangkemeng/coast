using RequirementTrackingSystem.Engines.SwaggerEngines;
using Microsoft.AspNetCore.Mvc;

namespace RequirementTrackingSystem.Controllers.Bases;

[Route("api/web/[controller]")]
[SwaggerApiGroup(SwaggerApiGroupNames.Web)]
public class WebBaseController : BaseController
{
}