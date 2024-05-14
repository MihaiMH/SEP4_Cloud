using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using weatherstation.Logic;

namespace weatherstation.Functions
{
    public class InsertData
    {
        private readonly ILogger<InsertData> _logger;

        public InsertData(ILogger<InsertData> logger)
        {
            _logger = logger;
        }

        [Function("InsertData")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData req)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject<dynamic>(requestBody);
            try
            {
                WeatherLogic.InsertWeatherData(data);
                return new OkResult();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
