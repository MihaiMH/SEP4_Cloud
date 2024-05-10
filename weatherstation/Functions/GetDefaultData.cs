using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
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
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req,
            FunctionContext executionContext)
        {
            var response = req.CreateResponse();
            try
            {
                CurrentWeatherDto dto = WeatherLogic.GetCurrentWeather();
                var json = JsonConvert.SerializeObject(dto);

                response.StatusCode = System.Net.HttpStatusCode.OK;
                await response.WriteStringAsync(json); // Write the JSON string to response
            }
            catch (Exception e)
            {
                _logger.LogError($"Error occurred: {e.Message}");
                response.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                await response.WriteStringAsync(e.Message); // Write the error message to response
            }

            return response;
        }
    }
}
