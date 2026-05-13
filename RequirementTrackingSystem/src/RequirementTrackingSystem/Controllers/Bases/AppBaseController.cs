using RequirementTrackingSystem.Engines.SwaggerEngines;
using Microsoft.AspNetCore.Mvc;

namespace RequirementTrackingSystem.Controllers.Bases;

[Route("api/app/[controller]")]
[SwaggerApiGroup(SwaggerApiGroupNames.App)]
public class AppBaseController : BaseController
{
}