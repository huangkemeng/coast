using RequirementTrackingSystem.Engines.SwaggerEngines;
using Microsoft.AspNetCore.Mvc;

namespace RequirementTrackingSystem.Controllers.Bases;

[Route("api/app/[controller]")]
[Route("api/web/[controller]")]
[SwaggerApiGroup(true)]
public class AllBaseController : BaseController
{
}