using RequirementTrackingSystem.Primary.Contracts.Requirements;
using RequirementTrackingSystem.Primary.Contracts.Bases;
using RequirementTrackingSystem.Realization.Bases;
using FluentValidation;
using Mediator.Net.Context;
using RequirementTrackingSystem.Infrastructure.DataPersistence.EfCore;
using RequirementTrackingSystem.Infrastructure.DataPersistence.EfCore.Entities;
using Microsoft.EntityFrameworkCore;

namespace RequirementTrackingSystem.Realization.Handlers.Requirements;

public class GetRequirementsHandler : IGetRequirementsContract
{
    private readonly ApplicationDbContext _dbContext;

    public GetRequirementsHandler(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void Validate(ContractValidator<GetRequirementsRequest> validator)
    {
        validator.RuleFor(e => e.PageIndex).GreaterThanOrEqualTo(1);
        validator.RuleFor(e => e.PageSize).InclusiveBetween(1, 100);
    }

    public async Task<GetRequirementsResponse> Handle(IReceiveContext<GetRequirementsRequest> context, CancellationToken cancellationToken)
    {
        var query = context.Message;
        var queryable = _dbContext.Requirements
            .Include(r => r.Follower)
            .Include(r => r.Project)
            .AsQueryable();

        if (!string.IsNullOrEmpty(query.Status))
        {
            var statusValues = query.Status.Split(',')
                .Select(s => Enum.Parse<RequirementStatus>(s.Trim()))
                .ToArray();
            queryable = queryable.Where(r => statusValues.Contains(r.Status));
        }

        if (query.FollowerId.HasValue)
        {
            queryable = queryable.Where(r => r.FollowerId == query.FollowerId.Value);
        }

        if (query.ProjectId.HasValue)
        {
            queryable = queryable.Where(r => r.ProjectId == query.ProjectId.Value);
        }

        if (query.PlanStartDateFrom.HasValue)
        {
            queryable = queryable.Where(r => r.PlanStartDate >= query.PlanStartDateFrom.Value);
        }

        if (query.PlanStartDateTo.HasValue)
        {
            queryable = queryable.Where(r => r.PlanStartDate <= query.PlanStartDateTo.Value);
        }

        if (query.PlanTestDateFrom.HasValue)
        {
            queryable = queryable.Where(r => r.PlanTestDate >= query.PlanTestDateFrom.Value);
        }

        if (query.PlanTestDateTo.HasValue)
        {
            queryable = queryable.Where(r => r.PlanTestDate <= query.PlanTestDateTo.Value);
        }

        var totalCount = await queryable.CountAsync(cancellationToken);

        if (!string.IsNullOrEmpty(query.SortBy))
        {
            queryable = query.SortBy?.ToLower() switch
            {
                "name" => query.SortOrder?.ToLower() == "desc"
                    ? queryable.OrderByDescending(r => r.Name)
                    : queryable.OrderBy(r => r.Name),
                "requirementno" => query.SortOrder?.ToLower() == "desc"
                    ? queryable.OrderByDescending(r => r.RequirementNo)
                    : queryable.OrderBy(r => r.RequirementNo),
                "plantestdate" => query.SortOrder?.ToLower() == "desc"
                    ? queryable.OrderByDescending(r => r.PlanTestDate)
                    : queryable.OrderBy(r => r.PlanTestDate),
                "priority" => query.SortOrder?.ToLower() == "desc"
                    ? queryable.OrderByDescending(r => r.Priority)
                    : queryable.OrderBy(r => r.Priority),
                "status" => query.SortOrder?.ToLower() == "desc"
                    ? queryable.OrderByDescending(r => r.Status)
                    : queryable.OrderBy(r => r.Status),
                "createdat" => query.SortOrder?.ToLower() == "desc"
                    ? queryable.OrderByDescending(r => r.CreatedAt)
                    : queryable.OrderBy(r => r.CreatedAt),
                _ => queryable.OrderByDescending(r => r.CreatedAt)
            };
        }
        else
        {
            queryable = queryable.OrderByDescending(r => r.CreatedAt);
        }

        var items = await queryable
            .Skip((query.PageIndex - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(r => new RequirementListItem
            {
                Id = r.Id,
                Name = r.Name,
                RequirementNo = r.RequirementNo,
                Status = r.Status,
                Progress = r.Progress,
                FollowerName = r.Follower != null ? r.Follower.RealName : null,
                ProjectName = r.Project != null ? r.Project.Name : null,
                Priority = r.Priority,
                PlanTestDate = r.PlanTestDate,
                CreatedAt = r.CreatedAt
            })
            .ToListAsync(cancellationToken);

        return new GetRequirementsResponse
        {
            Items = items,
            PageIndex = query.PageIndex,
            PageSize = query.PageSize,
            TotalCount = totalCount,
            Total = totalCount
        };
    }

    public void Test(TestContext<GetRequirementsRequest, GetRequirementsResponse> context)
    {
    }
}