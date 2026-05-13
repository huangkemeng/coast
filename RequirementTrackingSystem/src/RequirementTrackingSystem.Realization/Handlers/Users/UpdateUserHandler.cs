using RequirementTrackingSystem.Primary.Contracts.Users;
using RequirementTrackingSystem.Primary.Contracts.Bases;
using RequirementTrackingSystem.Realization.Bases;
using FluentValidation;
using Mediator.Net.Context;
using RequirementTrackingSystem.Infrastructure.DataPersistence.EfCore;
using RequirementTrackingSystem.Infrastructure.DataPersistence.EfCore.Entities;

namespace RequirementTrackingSystem.Realization.Handlers.Users;

public class UpdateUserHandler : IUpdateUserContract
{
    private readonly ApplicationDbContext _dbContext;

    public UpdateUserHandler(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void Validate(ContractValidator<UpdateUserCommand> validator)
    {
        validator.RuleFor(e => e.RealName).NotEmpty().MaximumLength(100);
    }

    public async Task<UpdateUserResponse> Handle(IReceiveContext<UpdateUserCommand> context, CancellationToken cancellationToken)
    {
        var command = context.Message;
        var user = await _dbContext.Users.FindAsync(new object[] { command.Id }, cancellationToken);

        if (user == null)
        {
            throw new BusinessException("用户不存在", BusinessExceptionTypeEnum.NotSpecified, "USER001");
        }

        user.RealName = command.RealName;
        user.Role = command.Role;
        user.Phone = command.Phone;
        user.Email = command.Email;
        user.IsEnabled = command.IsEnabled;

        return new UpdateUserResponse { Id = user.Id };
    }

    public void Test(TestContext<UpdateUserCommand, UpdateUserResponse> context)
    {
        context.NoDatabase = true;
    }
}