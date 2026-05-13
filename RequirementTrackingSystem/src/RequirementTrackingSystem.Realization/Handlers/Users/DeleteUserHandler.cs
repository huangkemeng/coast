using RequirementTrackingSystem.Primary.Contracts.Users;
using RequirementTrackingSystem.Primary.Contracts.Bases;
using RequirementTrackingSystem.Realization.Bases;
using FluentValidation;
using Mediator.Net.Context;
using RequirementTrackingSystem.Infrastructure.DataPersistence.EfCore;

namespace RequirementTrackingSystem.Realization.Handlers.Users;

public class DeleteUserHandler : IDeleteUserContract
{
    private readonly ApplicationDbContext _dbContext;

    public DeleteUserHandler(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void Validate(ContractValidator<DeleteUserCommand> validator)
    {
        validator.RuleFor(e => e.Id).GreaterThan(0);
    }

    public async Task Handle(IReceiveContext<DeleteUserCommand> context, CancellationToken cancellationToken)
    {
        var command = context.Message;
        var user = await _dbContext.Users.FindAsync(new object[] { command.Id }, cancellationToken);

        if (user == null)
        {
            throw new BusinessException("用户不存在", BusinessExceptionTypeEnum.NotSpecified, "USER001");
        }

        _dbContext.Users.Remove(user);
    }

    public void Test(TestContext<DeleteUserCommand> context)
    {
        context.NoDatabase = true;
    }
}