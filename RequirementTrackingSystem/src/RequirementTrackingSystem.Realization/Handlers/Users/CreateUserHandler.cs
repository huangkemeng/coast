using RequirementTrackingSystem.Primary.Contracts.Users;
using RequirementTrackingSystem.Primary.Contracts.Bases;
using RequirementTrackingSystem.Realization.Bases;
using FluentValidation;
using Mediator.Net.Context;
using RequirementTrackingSystem.Infrastructure.DataPersistence.EfCore;
using RequirementTrackingSystem.Infrastructure.DataPersistence.EfCore.Entities;

namespace RequirementTrackingSystem.Realization.Handlers.Users;

public class CreateUserHandler : ICreateUserContract
{
    private readonly ApplicationDbContext _dbContext;

    public CreateUserHandler(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void Validate(ContractValidator<CreateUserCommand> validator)
    {
        validator.RuleFor(e => e.Username).NotEmpty().MaximumLength(50);
        validator.RuleFor(e => e.RealName).NotEmpty().MaximumLength(100);
    }

    public async Task<CreateUserResponse> Handle(IReceiveContext<CreateUserCommand> context, CancellationToken cancellationToken)
    {
        var command = context.Message;
        var user = new User
        {
            Username = command.Username,
            RealName = command.RealName,
            Role = command.Role,
            Phone = command.Phone,
            Email = command.Email,
            IsEnabled = command.IsEnabled,
            CreatedAt = DateTime.UtcNow
        };

        await _dbContext.Users.AddAsync(user, cancellationToken);
        return new CreateUserResponse { Id = user.Id };
    }

    public void Test(TestContext<CreateUserCommand, CreateUserResponse> context)
    {
        context.NoDatabase = true;
    }
}