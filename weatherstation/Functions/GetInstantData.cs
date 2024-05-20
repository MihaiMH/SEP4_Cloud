using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Text;
using weatherstation.Application.Logic;
using weatherstation.Utils;
using Microsoft.Azure.Functions.Worker.Http;
using weatherstation.Domain.DTOs;
using weatherstation.Application.LogicInterfaces;
namespace weatherstation.Functions
{
    public class GetInstantData
    {
        private readonly ILogger<GetInstantData> _logger;
        private IWeatherLogic weatherLogic;

        public GetInstantData(ILogger<GetInstantData> logger, IWeatherLogic weatherLogic)
        {
            _logger = logger;
            this.weatherLogic = weatherLogic;
        }
        [Function("GetInstantData")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequestData req)
        {
            var res = req.CreateResponse();

            try
            {
                Token decoder = new Token();
                string token = decoder.Extract(req);

                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                JObject? json = JsonConvert.DeserializeObject<JObject>(requestBody);

                if (json == null)
                {
                    json = new JObject();
                }

                string result = "";
                if (!decoder.IsTokenValid(token))
                {
                    res.StatusCode = System.Net.HttpStatusCode.Unauthorized;
                    var msg = JsonConvert.SerializeObject(new { msg = "NOT AUTHORIZED" }, Formatting.Indented);
                    res.Body = new MemoryStream(Encoding.UTF8.GetBytes(msg));
                    return res;
                }
                else
                {
                    CurrentWeatherDto? dto = await weatherLogic.GetInstantWeather();
                    res.StatusCode = System.Net.HttpStatusCode.OK;
                    res.Body = new MemoryStream(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(dto, Formatting.Indented)));
                    return res;
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "An error occurred while getting instant data.");
                res.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                res.Body = new MemoryStream(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new { error = "An error occurred while getting instant data." }, Formatting.Indented)));
                return res;
            }
        }
    }
}
