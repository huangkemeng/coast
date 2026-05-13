using RequirementTrackingSystem.Primary.Contracts.Projects;
using RequirementTrackingSystem.Primary.Contracts.Bases;
using RequirementTrackingSystem.Realization.Bases;
using FluentValidation;
using Mediator.Net.Context;
using RequirementTrackingSystem.Infrastructure.DataPersistence.EfCore;
using RequirementTrackingSystem.Infrastructure.DataPersistence.EfCore.Entities;

namespace RequirementTrackingSystem.Realization.Handlers.Projects;

public class UpdateProjectHandler : IUpdateProjectContract
{
    private readonly ApplicationDbContext _dbContext;

    public UpdateProjectHandler(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void Validate(ContractValidator<UpdateProjectCommand> validator)
    {
        validator.RuleFor(e => e.Name).NotEmpty().MaximumLength(100);
        validator.RuleFor(e => e.ManagerId).GreaterThan(0);
    }

    public async Task<UpdateProjectResponse> Handle(IReceiveContext<UpdateProjectCommand> context, CancellationToken cancellationToken)
    {
        var command = context.Message;
        var project = await _dbContext.Projects.FindAsync(new object[] { command.Id }, cancellationToken);

        if (project == null)
        {
            throw new BusinessException("项目不存在", BusinessExceptionTypeEnum.NotSpecified, "PROJ001");
        }

        project.Name = command.Name;
        project.ManagerId = command.ManagerId;
        project.Description = command.Description;

        return new UpdateProjectResponse { Id = project.Id };
    }

    public void Test(TestContext<UpdateProjectCommand, UpdateProjectResponse> context)
    {
        context.NoDatabase = true;
    }
}