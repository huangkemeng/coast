using RequirementTrackingSystem.Primary.Bases;
using RequirementTrackingSystem.Primary.Contracts.Bases;
using RequirementTrackingSystem.Infrastructure.DataPersistence.EfCore.Entities;
using Mediator.Net.Contracts;

namespace RequirementTrackingSystem.Primary.Contracts.Robots;

public class GetRobotsRequest : IRequest
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

public class RobotListItem : IMapFrom<Robot>
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string WebhookUrl { get; set; } = string.Empty;
    public string? GroupName { get; set; }
    public bool IsEnabled { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class GetRobotsResponse : IResponse, IPaginated
{
    public List<RobotListItem> Items { get; set; } = new();
    public int PageIndex { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    public int Total { get; set; }
}

public interface IGetRobotsContract : IRequestContract<GetRobotsRequest, GetRobotsResponse>
{
}

public class GetRobotByIdQuery : IRequest
{
    public int Id { get; set; }
}

public class RobotDetailResponse : IResponse, IMapFrom<Robot>
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string WebhookUrl { get; set; } = string.Empty;
    public string? GroupName { get; set; }
    public bool IsEnabled { get; set; }
    public DateTime CreatedAt { get; set; }
}

public interface IGetRobotByIdContract : IRequestContract<GetRobotByIdQuery, RobotDetailResponse>
{
}

public class TestRobotCommand : ICommand
{
    public int Id { get; set; }
}

public class TestRobotResponse : IResponse
{
    public bool Success { get; set; }
    public string? Message { get; set; }
}

public interface ITestRobotContract : ICommandContract<TestRobotCommand, TestRobotResponse>
{
}