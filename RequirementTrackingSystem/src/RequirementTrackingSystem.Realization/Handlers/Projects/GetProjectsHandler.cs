using RequirementTrackingSystem.Primary.Contracts.Projects;
using RequirementTrackingSystem.Primary.Contracts.Bases;
using RequirementTrackingSystem.Realization.Bases;
using FluentValidation;
using Mediator.Net.Context;
using RequirementTrackingSystem.Infrastructure.DataPersistence.EfCore;
using Microsoft.EntityFrameworkCore;

namespace RequirementTrackingSystem.Realization.Handlers.Projects;

public class GetProjectsHandler : IGetProjectsContract
{
    private readonly ApplicationDbContext _dbContext;

    public GetProjectsHandler(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void Validate(ContractValidator<GetProjectsRequest> validator)
    {
        validator.RuleFor(e => e.PageIndex).GreaterThanOrEqualTo(1);
        validator.RuleFor(e => e.PageSize).InclusiveBetween(1, 100);
    }

    public async Task<GetProjectsResponse> Handle(IReceiveContext<GetProjectsRequest> context, CancellationToken cancellationToken)
    {
        var query = context.Message;
        var queryable = _dbContext.Projects.Include(p => p.Manager).AsQueryable();

        var totalCount = await queryable.CountAsync(cancellationToken);
        var items = await queryable
            .OrderByDescending(p => p.CreatedAt)
            .Skip((query.PageIndex - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(p => new ProjectListItem
            {
                Id = p.Id,
                Name = p.Name,
                Code = p.Code,
                ManagerName = p.Manager != null ? p.Manager.RealName : null,
                Description = p.Description,
                CreatedAt = p.CreatedAt
            })
            .ToListAsync(cancellationToken);

        return new GetProjectsResponse
        {
            Items = items,
            PageIndex = query.PageIndex,
            PageSize = query.PageSize,
            TotalCount = totalCount,
            Total = totalCount
        };
    }

    public void Test(TestContext<GetProjectsRequest, GetProjectsResponse> context)
    {
    }
}

public class GetProjectByIdHandler : IGetProjectByIdContract
{
    private readonly ApplicationDbContext _dbContext;

    public GetProjectByIdHandler(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void Validate(ContractValidator<GetProjectByIdQuery> validator)
    {
        validator.RuleFor(e => e.Id).GreaterThan(0);
    }

    public async Task<ProjectDetailResponse> Handle(IReceiveContext<GetProjectByIdQuery> context, CancellationToken cancellationToken)
    {
        var query = context.Message;
        var project = await _dbContext.Projects.Include(p => p.Manager).FirstOrDefaultAsync(p => p.Id == query.Id, cancellationToken);

        if (project == null)
        {
            throw new BusinessException("项目不存在", BusinessExceptionTypeEnum.NotSpecified, "PROJ001");
        }

        return new ProjectDetailResponse
        {
            Id = project.Id,
            Name = project.Name,
            Code = project.Code,
            ManagerId = project.ManagerId,
            ManagerName = project.Manager?.RealName,
            Description = project.Description,
            CreatedAt = project.CreatedAt
        };
    }

    public void Test(TestContext<GetProjectByIdQuery, ProjectDetailResponse> context)
    {
    }
}