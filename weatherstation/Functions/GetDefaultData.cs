using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
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
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequest req)
        {
            try
            {
                List<CurrentWeatherDto> dto = await WeatherLogic.GetAllWeather();

                return new OkObjectResult(dto);
            } 
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
