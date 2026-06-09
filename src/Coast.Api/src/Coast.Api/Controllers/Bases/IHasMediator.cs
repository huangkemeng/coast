using Mediator.Net;

namespace Coast.Api.Controllers.Bases;

public interface IHasMediator
{
    IMediator Mediator { get; set; }
}