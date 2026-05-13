using RequirementTrackingSystem.Primary.Contracts.Projects;
using RequirementTrackingSystem.Primary.Contracts.Bases;
using RequirementTrackingSystem.Realization.Bases;
using FluentValidation;
using Mediator.Net.Context;
using RequirementTrackingSystem.Infrastructure.DataPersistence.EfCore;

namespace RequirementTrackingSystem.Realization.Handlers.Projects;

public class DeleteProjectHandler : IDeleteProjectContract
{
    private readonly ApplicationDbContext _dbContext;

    public DeleteProjectHandler(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void Validate(ContractValidator<DeleteProjectCommand> validator)
    {
        validator.RuleFor(e => e.Id).GreaterThan(0);
    }

    public async Task Handle(IReceiveContext<DeleteProjectCommand> context, CancellationToken cancellationToken)
    {
        var command = context.Message;
        var project = await _dbContext.Projects.FindAsync(new object[] { command.Id }, cancellationToken);

        if (project == null)
        {
            throw new BusinessException("项目不存在", BusinessExceptionTypeEnum.NotSpecified, "PROJ001");
        }

        _dbContext.Projects.Remove(project);
    }

    public void Test(TestContext<DeleteProjectCommand> context)
    {
        context.NoDatabase = true;
    }
}