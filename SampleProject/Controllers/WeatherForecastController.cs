using Microsoft.AspNetCore.Mvc;
using SampleProject.Domain.Interfaces.Application;

namespace SampleProject.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IOrderApplication _appService;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IOrderApplication appService)
        {
            _logger = logger;
            _appService = appService;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public async Task<IEnumerable<WeatherForecast>> Get()
        {
            // ´ú¸Õ¥Î
            var result = await _appService.Create(new API.Model.Order.Request.CreateRequest { });

            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }
}
