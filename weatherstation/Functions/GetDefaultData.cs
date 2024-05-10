using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using weatherstation.Domain.DTOs;
using weatherstation.Logic;

namespace weatherstation.Functions
{
    public class GetDefaultData
    {
        private readonly ILogger<GetDefaultData> _logger;

        public GetDefaultData(ILogger<GetDefaultData> logger)
        {
            _logger = logger;
        }

        [Function("GetDefaultData")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req)
        {

            CurrentWeatherDto dto = WeatherLogic.GetCurrentWeather();

            return new OkObjectResult(dto);
        }
    }
}
