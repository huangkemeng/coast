using Mediator.Net.Contracts;

namespace Coast.Api.Primary.Bases;

public class BaseResponse<T> : IResponse
{
    public BaseResponse(T data)
    {
        Data = data;
    }
    
    public T Data { get; set; }
}