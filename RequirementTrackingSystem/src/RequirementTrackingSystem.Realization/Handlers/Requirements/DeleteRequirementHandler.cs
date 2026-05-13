using RequirementTrackingSystem.Primary.Contracts.Requirements;
using RequirementTrackingSystem.Primary.Contracts.Bases;
using RequirementTrackingSystem.Realization.Bases;
using FluentValidation;
using Mediator.Net.Context;
using RequirementTrackingSystem.Infrastructure.DataPersistence.EfCore;

namespace RequirementTrackingSystem.Realization.Handlers.Requirements;

public class DeleteRequirementHandler : IDeleteRequirementContract
{
    private readonly ApplicationDbContext _dbContext;

    public DeleteRequirementHandler(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void Validate(ContractValidator<DeleteRequirementCommand> validator)
    {
        validator.RuleFor(e => e.Id).GreaterThan(0);
    }

    public async Task Handle(IReceiveContext<DeleteRequirementCommand> context, CancellationToken cancellationToken)
    {
        var command = context.Message;
        var requirement = await _dbContext.Requirements.FindAsync(new object[] { command.Id }, cancellationToken);

        if (requirement == null)
        {
            throw new BusinessException("需求不存在", BusinessExceptionTypeEnum.NotSpecified, "REQ001");
        }

        _dbContext.Requirements.Remove(requirement);
    }

    public void Test(TestContext<DeleteRequirementCommand> context)
    {
        context.NoDatabase = true;
    }
}