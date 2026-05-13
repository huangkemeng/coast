using RequirementTrackingSystem.Primary.Contracts.Projects;
using RequirementTrackingSystem.Primary.Contracts.Bases;
using RequirementTrackingSystem.Realization.Bases;
using FluentValidation;
using Mediator.Net.Context;
using RequirementTrackingSystem.Infrastructure.DataPersistence.EfCore;
using RequirementTrackingSystem.Infrastructure.DataPersistence.EfCore.Entities;

namespace RequirementTrackingSystem.Realization.Handlers.Projects;

public class CreateProjectHandler : ICreateProjectContract
{
    private readonly ApplicationDbContext _dbContext;

    public CreateProjectHandler(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void Validate(ContractValidator<CreateProjectCommand> validator)
    {
        validator.RuleFor(e => e.Name).NotEmpty().MaximumLength(100);
        validator.RuleFor(e => e.Code).NotEmpty().MaximumLength(50);
        validator.RuleFor(e => e.ManagerId).GreaterThan(0);
    }

    public async Task<CreateProjectResponse> Handle(IReceiveContext<CreateProjectCommand> context, CancellationToken cancellationToken)
    {
        var command = context.Message;
        var project = new Project
        {
            Name = command.Name,
            Code = command.Code,
            ManagerId = command.ManagerId,
            Description = command.Description,
            CreatedAt = DateTime.UtcNow
        };

        await _dbContext.Projects.AddAsync(project, cancellationToken);
        return new CreateProjectResponse { Id = project.Id };
    }

    public void Test(TestContext<CreateProjectCommand, CreateProjectResponse> context)
    {
        context.NoDatabase = true;
    }
}