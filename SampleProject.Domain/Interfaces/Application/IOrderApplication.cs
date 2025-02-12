using SampleProject.API.Model.Base;
using SampleProject.API.Model.Order.Request;
using SampleProject.API.Model.Order.Response;

namespace SampleProject.Domain.Interfaces.Application
{
    public interface IOrderApplication
    {
        Task<ApiResult<GetOrderResponse>> Get(GetOrderRequest request);

        Task<ApiResult<CreateResponse>> Create(CreateRequest request);
    }
}