using RequirementTrackingSystem.Controllers.Bases;
using RequirementTrackingSystem.Primary.Contracts.Users;
using Microsoft.AspNetCore.Mvc;

namespace RequirementTrackingSystem.Controllers;

public class UsersController : WebBaseController
{
    [HttpGet]
    [ProducesResponseType(typeof(GetUsersResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetListAsync([FromQuery] GetUsersRequest request, CancellationToken cancellationToken)
    {
        var response = await Mediator.RequestAsync<GetUsersRequest, GetUsersResponse>(request, cancellationToken);
        return Ok(response);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(UserDetailResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        var response = await Mediator.RequestAsync<GetUserByIdQuery, UserDetailResponse>(
            new GetUserByIdQuery { Id = id }, cancellationToken);
        return Ok(response);
    }

    [HttpPost]
    [ProducesResponseType(typeof(CreateUserResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateAsync([FromBody] CreateUserCommand command, CancellationToken cancellationToken)
    {
        var response = await Mediator.SendAsync<CreateUserCommand, CreateUserResponse>(command, cancellationToken);
        return Ok(response);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(UpdateUserResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateAsync(int id, [FromBody] UpdateUserCommand command, CancellationToken cancellationToken)
    {
        command.Id = id;
        var response = await Mediator.SendAsync<UpdateUserCommand, UpdateUserResponse>(command, cancellationToken);
        return Ok(response);
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        await Mediator.SendAsync<DeleteUserCommand>(new DeleteUserCommand { Id = id }, cancellationToken);
        return NoContent();
    }
}