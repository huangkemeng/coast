using Mediator.Net;

namespace RequirementTrackingSystem.Controllers.Bases;

public interface IHasMediator
{
    IMediator Mediator { get; set; }
}