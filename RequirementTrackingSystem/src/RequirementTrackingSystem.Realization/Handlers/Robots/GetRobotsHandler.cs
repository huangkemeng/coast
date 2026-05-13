using RequirementTrackingSystem.Primary.Contracts.Robots;
using RequirementTrackingSystem.Primary.Contracts.Bases;
using RequirementTrackingSystem.Realization.Bases;
using FluentValidation;
using Mediator.Net.Context;
using RequirementTrackingSystem.Infrastructure.DataPersistence.EfCore;
using RequirementTrackingSystem.Infrastructure.DataPersistence.EfCore.Entities;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Json;

namespace RequirementTrackingSystem.Realization.Handlers.Robots;

public class GetRobotsHandler : IGetRobotsContract
{
    private readonly ApplicationDbContext _dbContext;

    public GetRobotsHandler(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void Validate(ContractValidator<GetRobotsRequest> validator)
    {
        validator.RuleFor(e => e.PageIndex).GreaterThanOrEqualTo(1);
        validator.RuleFor(e => e.PageSize).InclusiveBetween(1, 100);
    }

    public async Task<GetRobotsResponse> Handle(IReceiveContext<GetRobotsRequest> context, CancellationToken cancellationToken)
    {
        var query = context.Message;
        var queryable = _dbContext.Robots.AsQueryable();

        var totalCount = await queryable.CountAsync(cancellationToken);
        var items = await queryable
            .OrderByDescending(r => r.CreatedAt)
            .Skip((query.PageIndex - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(r => new RobotListItem
            {
                Id = r.Id,
                Name = r.Name,
                WebhookUrl = r.WebhookUrl,
                GroupName = r.GroupName,
                IsEnabled = r.IsEnabled,
                CreatedAt = r.CreatedAt
            })
            .ToListAsync(cancellationToken);

        return new GetRobotsResponse
        {
            Items = items,
            PageIndex = query.PageIndex,
            PageSize = query.PageSize,
            TotalCount = totalCount,
            Total = totalCount
        };
    }

    public void Test(TestContext<GetRobotsRequest, GetRobotsResponse> context)
    {
    }
}

public class GetRobotByIdHandler : IGetRobotByIdContract
{
    private readonly ApplicationDbContext _dbContext;

    public GetRobotByIdHandler(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void Validate(ContractValidator<GetRobotByIdQuery> validator)
    {
        validator.RuleFor(e => e.Id).GreaterThan(0);
    }

    public async Task<RobotDetailResponse> Handle(IReceiveContext<GetRobotByIdQuery> context, CancellationToken cancellationToken)
    {
        var query = context.Message;
        var robot = await _dbContext.Robots.FirstOrDefaultAsync(r => r.Id == query.Id, cancellationToken);

        if (robot == null)
        {
            throw new BusinessException("机器人不存在", BusinessExceptionTypeEnum.NotSpecified, "ROBOT001");
        }

        return new RobotDetailResponse
        {
            Id = robot.Id,
            Name = robot.Name,
            WebhookUrl = robot.WebhookUrl,
            GroupName = robot.GroupName,
            IsEnabled = robot.IsEnabled,
            CreatedAt = robot.CreatedAt
        };
    }

    public void Test(TestContext<GetRobotByIdQuery, RobotDetailResponse> context)
    {
    }
}

public class TestRobotHandler : ITestRobotContract
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IHttpClientFactory _httpClientFactory;

    public TestRobotHandler(ApplicationDbContext dbContext, IHttpClientFactory httpClientFactory)
    {
        _dbContext = dbContext;
        _httpClientFactory = httpClientFactory;
    }

    public void Validate(ContractValidator<TestRobotCommand> validator)
    {
        validator.RuleFor(e => e.Id).GreaterThan(0);
    }

    public async Task<TestRobotResponse> Handle(IReceiveContext<TestRobotCommand> context, CancellationToken cancellationToken)
    {
        var command = context.Message;
        var robot = await _dbContext.Robots.FindAsync(new object[] { command.Id }, cancellationToken);

        if (robot == null)
        {
            throw new BusinessException("机器人不存在", BusinessExceptionTypeEnum.NotSpecified, "ROBOT001");
        }

        try
        {
            var httpClient = _httpClientFactory.CreateClient();
            var payload = new { msgtype = "text", text = new { content = "这是一条测试消息，用于验证机器人配置是否正确。" } };
            var response = await httpClient.PostAsJsonAsync(robot.WebhookUrl, payload, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                return new TestRobotResponse { Success = true, Message = "测试消息发送成功" };
            }
            else
            {
                return new TestRobotResponse { Success = false, Message = $"发送失败，HTTP状态码: {response.StatusCode}" };
            }
        }
        catch (Exception ex)
        {
            return new TestRobotResponse { Success = false, Message = $"发送失败: {ex.Message}" };
        }
    }

    public void Test(TestContext<TestRobotCommand, TestRobotResponse> context)
    {
        context.NoDatabase = true;
    }
}