using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Ocsp;
using System.Text;
using weatherstation.Application.Logic;
using weatherstation.Domain.DTOs;
using weatherstation.Logic;
using weatherstation.Utils;

namespace weatherstation.Functions
{
    public class GetWeatherStatistics
    {
        private readonly ILogger<GetWeatherStatistics> _logger;

        public GetWeatherStatistics(ILogger<GetWeatherStatistics> logger)
        {
            _logger = logger;
        }

        [Function("GetWeatherStatistics")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "GetWeatherStatistics/{period}")] HttpRequestData reqData, string period)
        {
            var res = reqData.CreateResponse();
            try
            {

                TokenDecoder decoder = new TokenDecoder();
                string token = decoder.Extract(reqData);

                string requestBody = await new StreamReader(reqData.Body).ReadToEndAsync();
                JObject json = JsonConvert.DeserializeObject<JObject>(requestBody);

                if (json == null)
                {
                    json = new JObject();
                }

                string result = "";
                if (token == null)
                {
                    res.StatusCode = System.Net.HttpStatusCode.Unauthorized;
                    var msg = JsonConvert.SerializeObject(new { msg = "Unauthorized" }, Formatting.Indented);
                    res.Body = new MemoryStream(Encoding.UTF8.GetBytes(msg));
                    return res;
                }
                else
                {
                    dynamic dto = period.ToLower() switch
                    {
                        "week" => await WeatherLogic.GetStatistics("WEEK"),
                        "month" => await WeatherLogic.GetStatistics("MONTH"),
                        "year" => await WeatherLogic.GetStatistics("YEAR"),
                        _ => throw new ArgumentException("Invalid period specified")
                    };

                    res.StatusCode = System.Net.HttpStatusCode.OK;
                    res.Body = new MemoryStream(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(dto, Formatting.Indented)));
                    return res;
                }
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex.Message);
                res.StatusCode = System.Net.HttpStatusCode.BadRequest;
                res.Body = new MemoryStream(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new { error = ex.Message }, Formatting.Indented)));
                return res;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                res.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                res.Body = new MemoryStream(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new { error = "There was an error while retrieving data." }, Formatting.Indented)));
                return res;
            }
            return res;
        }
    }
}
