using RequirementTrackingSystem.Controllers.Bases;
using RequirementTrackingSystem.Primary.Contracts.Projects;
using Microsoft.AspNetCore.Mvc;

namespace RequirementTrackingSystem.Controllers;

public class ProjectsController : WebBaseController
{
    [HttpGet]
    [ProducesResponseType(typeof(GetProjectsResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetListAsync([FromQuery] GetProjectsRequest request, CancellationToken cancellationToken)
    {
        var response = await Mediator.RequestAsync<GetProjectsRequest, GetProjectsResponse>(request, cancellationToken);
        return Ok(response);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ProjectDetailResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        var response = await Mediator.RequestAsync<GetProjectByIdQuery, ProjectDetailResponse>(
            new GetProjectByIdQuery { Id = id }, cancellationToken);
        return Ok(response);
    }

    [HttpPost]
    [ProducesResponseType(typeof(CreateProjectResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateAsync([FromBody] CreateProjectCommand command, CancellationToken cancellationToken)
    {
        var response = await Mediator.SendAsync<CreateProjectCommand, CreateProjectResponse>(command, cancellationToken);
        return Ok(response);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(UpdateProjectResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateAsync(int id, [FromBody] UpdateProjectCommand command, CancellationToken cancellationToken)
    {
        command.Id = id;
        var response = await Mediator.SendAsync<UpdateProjectCommand, UpdateProjectResponse>(command, cancellationToken);
        return Ok(response);
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        await Mediator.SendAsync<DeleteProjectCommand>(new DeleteProjectCommand { Id = id }, cancellationToken);
        return NoContent();
    }
}