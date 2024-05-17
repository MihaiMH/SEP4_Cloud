using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;
using weatherstation.Domain.DTOs;
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
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData req)
        {
            var res = req.CreateResponse(); 
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            JObject json = JsonConvert.DeserializeObject<JObject>(requestBody);
            try
            {
                CurrentWeatherDto dto = await WeatherLogic.InsertWeatherData(json);
                res.StatusCode = System.Net.HttpStatusCode.OK;
                res.Body = new MemoryStream(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new { data = dto, msg = "Data inserted succesfully" }, Formatting.Indented)));
                return res;

            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "An error occurred while inserting.");
                res.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                res.Body = new MemoryStream(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new { error = "An error occured while inserting data" }, Formatting.Indented)));
                return res;
            }
        }
    }
}
