using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Text;
using weatherstation.Domain.DTOs;
using weatherstation.Application.Logic;


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
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequestData reqData)
        {
            var res = reqData.CreateResponse();

            try
            {
                List<CurrentWeatherDto> dto = await WeatherLogic.GetAllWeather();
                res.StatusCode = System.Net.HttpStatusCode.OK;
                res.Body = new MemoryStream(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(dto, Formatting.Indented)));
                return res;

            } 
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                res.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                res.Body = new MemoryStream(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new { error = "There was an error while retrieving data." }, Formatting.Indented)));
                return res;
            }
        }
    }
}
