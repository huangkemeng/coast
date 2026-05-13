using RequirementTrackingSystem.Primary.Contracts.Users;
using RequirementTrackingSystem.Primary.Contracts.Bases;
using RequirementTrackingSystem.Realization.Bases;
using FluentValidation;
using Mediator.Net.Context;
using RequirementTrackingSystem.Infrastructure.DataPersistence.EfCore;
using RequirementTrackingSystem.Infrastructure.DataPersistence.EfCore.Entities;
using Microsoft.EntityFrameworkCore;

namespace RequirementTrackingSystem.Realization.Handlers.Users;

public class GetUsersHandler : IGetUsersContract
{
    private readonly ApplicationDbContext _dbContext;

    public GetUsersHandler(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void Validate(ContractValidator<GetUsersRequest> validator)
    {
        validator.RuleFor(e => e.PageIndex).GreaterThanOrEqualTo(1);
        validator.RuleFor(e => e.PageSize).InclusiveBetween(1, 100);
    }

    public async Task<GetUsersResponse> Handle(IReceiveContext<GetUsersRequest> context, CancellationToken cancellationToken)
    {
        var query = context.Message;
        var queryable = _dbContext.Users.AsQueryable();

        var totalCount = await queryable.CountAsync(cancellationToken);
        var items = await queryable
            .OrderByDescending(u => u.CreatedAt)
            .Skip((query.PageIndex - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(u => new UserListItem
            {
                Id = u.Id,
                Username = u.Username,
                RealName = u.RealName,
                Role = u.Role,
                Phone = u.Phone,
                Email = u.Email,
                IsEnabled = u.IsEnabled,
                CreatedAt = u.CreatedAt
            })
            .ToListAsync(cancellationToken);

        return new GetUsersResponse
        {
            Items = items,
            PageIndex = query.PageIndex,
            PageSize = query.PageSize,
            TotalCount = totalCount,
            Total = totalCount
        };
    }

    public void Test(TestContext<GetUsersRequest, GetUsersResponse> context)
    {
    }
}

public class GetUserByIdHandler : IGetUserByIdContract
{
    private readonly ApplicationDbContext _dbContext;

    public GetUserByIdHandler(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void Validate(ContractValidator<GetUserByIdQuery> validator)
    {
        validator.RuleFor(e => e.Id).GreaterThan(0);
    }

    public async Task<UserDetailResponse> Handle(IReceiveContext<GetUserByIdQuery> context, CancellationToken cancellationToken)
    {
        var query = context.Message;
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == query.Id, cancellationToken);

        if (user == null)
        {
            throw new BusinessException("用户不存在", BusinessExceptionTypeEnum.NotSpecified, "USER001");
        }

        return new UserDetailResponse
        {
            Id = user.Id,
            Username = user.Username,
            RealName = user.RealName,
            Role = user.Role,
            Phone = user.Phone,
            Email = user.Email,
            IsEnabled = user.IsEnabled,
            CreatedAt = user.CreatedAt
        };
    }

    public void Test(TestContext<GetUserByIdQuery, UserDetailResponse> context)
    {
    }
}