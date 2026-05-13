using RequirementTrackingSystem.Controllers.Bases;
using RequirementTrackingSystem.Primary.Contracts.Requirements;
using Microsoft.AspNetCore.Mvc;

namespace RequirementTrackingSystem.Controllers;

public class RequirementsController : WebBaseController
{
    [HttpGet]
    [ProducesResponseType(typeof(GetRequirementsResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetListAsync([FromQuery] GetRequirementsRequest request, CancellationToken cancellationToken)
    {
        var response = await Mediator.RequestAsync<GetRequirementsRequest, GetRequirementsResponse>(request, cancellationToken);
        return Ok(response);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(RequirementDetailResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        var response = await Mediator.RequestAsync<GetRequirementByIdQuery, RequirementDetailResponse>(
            new GetRequirementByIdQuery { Id = id }, cancellationToken);
        return Ok(response);
    }

    [HttpPost]
    [ProducesResponseType(typeof(CreateRequirementResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateAsync([FromBody] CreateRequirementCommand command, CancellationToken cancellationToken)
    {
        var response = await Mediator.SendAsync<CreateRequirementCommand, CreateRequirementResponse>(command, cancellationToken);
        return Ok(response);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(UpdateRequirementResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateAsync(int id, [FromBody] UpdateRequirementCommand command, CancellationToken cancellationToken)
    {
        command.Id = id;
        var response = await Mediator.SendAsync<UpdateRequirementCommand, UpdateRequirementResponse>(command, cancellationToken);
        return Ok(response);
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        await Mediator.SendAsync<DeleteRequirementCommand>(new DeleteRequirementCommand { Id = id }, cancellationToken);
        return NoContent();
    }

    [HttpPut("{id:int}/status")]
    [ProducesResponseType(typeof(ChangeRequirementStatusResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> ChangeStatusAsync(int id, [FromBody] ChangeRequirementStatusCommand command, CancellationToken cancellationToken)
    {
        command.Id = id;
        var response = await Mediator.SendAsync<ChangeRequirementStatusCommand, ChangeRequirementStatusResponse>(command, cancellationToken);
        return Ok(response);
    }
}