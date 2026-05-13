using RequirementTrackingSystem.Controllers.Bases;
using RequirementTrackingSystem.Primary.Contracts.Notifications;
using Microsoft.AspNetCore.Mvc;

namespace RequirementTrackingSystem.Controllers;

public class NotificationsController : WebBaseController
{
    [HttpGet]
    [ProducesResponseType(typeof(GetNotificationLogsResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetListAsync([FromQuery] GetNotificationLogsRequest request, CancellationToken cancellationToken)
    {
        var response = await Mediator.RequestAsync<GetNotificationLogsRequest, GetNotificationLogsResponse>(request, cancellationToken);
        return Ok(response);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(NotificationLogDetailResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        var response = await Mediator.RequestAsync<GetNotificationLogByIdQuery, NotificationLogDetailResponse>(
            new GetNotificationLogByIdQuery { Id = id }, cancellationToken);
        return Ok(response);
    }
}