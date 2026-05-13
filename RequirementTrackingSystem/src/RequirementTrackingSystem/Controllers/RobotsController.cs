using RequirementTrackingSystem.Controllers.Bases;
using RequirementTrackingSystem.Primary.Contracts.Robots;
using Microsoft.AspNetCore.Mvc;

namespace RequirementTrackingSystem.Controllers;

public class RobotsController : WebBaseController
{
    [HttpGet]
    [ProducesResponseType(typeof(GetRobotsResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetListAsync([FromQuery] GetRobotsRequest request, CancellationToken cancellationToken)
    {
        var response = await Mediator.RequestAsync<GetRobotsRequest, GetRobotsResponse>(request, cancellationToken);
        return Ok(response);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(RobotDetailResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        var response = await Mediator.RequestAsync<GetRobotByIdQuery, RobotDetailResponse>(
            new GetRobotByIdQuery { Id = id }, cancellationToken);
        return Ok(response);
    }

    [HttpPost]
    [ProducesResponseType(typeof(CreateRobotResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateAsync([FromBody] CreateRobotCommand command, CancellationToken cancellationToken)
    {
        var response = await Mediator.SendAsync<CreateRobotCommand, CreateRobotResponse>(command, cancellationToken);
        return Ok(response);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(UpdateRobotResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateAsync(int id, [FromBody] UpdateRobotCommand command, CancellationToken cancellationToken)
    {
        command.Id = id;
        var response = await Mediator.SendAsync<UpdateRobotCommand, UpdateRobotResponse>(command, cancellationToken);
        return Ok(response);
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        await Mediator.SendAsync<DeleteRobotCommand>(new DeleteRobotCommand { Id = id }, cancellationToken);
        return NoContent();
    }

    [HttpPost("{id:int}/test")]
    [ProducesResponseType(typeof(TestRobotResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> TestAsync(int id, CancellationToken cancellationToken)
    {
        var response = await Mediator.SendAsync<TestRobotCommand, TestRobotResponse>(
            new TestRobotCommand { Id = id }, cancellationToken);
        return Ok(response);
    }
}