using Microsoft.AspNetCore.Mvc;
using SampleProject.API.Model.Base;
using SampleProject.API.Model.Order.Request;
using SampleProject.API.Model.Order.Response;
using SampleProject.Domain.Interfaces.Application;

namespace SampleProject.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly ILogger<OrderController> _logger;
        private readonly IOrderApplication _appService;

        public OrderController(ILogger<OrderController> logger, IOrderApplication appService)
        {
            _logger = logger;
            _appService = appService;
        }

        [HttpGet]
        public async Task<ApiResult<GetOrderResponse>> Get([FromQuery] GetOrderRequest request)
        {
            return await _appService.Get(request);
        }

        [HttpPost]
        public async Task<ApiResult<CreateResponse>> Post(CreateRequest request)
        {
            return await _appService.Create(request);
        }
    }
}
