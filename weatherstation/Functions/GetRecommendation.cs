using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using weatherstation.Application.Logic;
using weatherstation.Utils;

namespace weatherstation.Functions
{
    public class GetRecommendation
    {
        private readonly ILogger<GetRecommendation> _logger;

        public GetRecommendation(ILogger<GetRecommendation> logger)
        {
            _logger = logger;
        }

        [Function("GetRecommendation")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData reqData)
        {
            var response = reqData.CreateResponse();

            try
            {
                TokenDecoder decoder = new TokenDecoder();

                // Check if the Authorization header is present
                string token = decoder.Extract(reqData);

                _logger.LogError(token);

                // Read the request body
                string requestBody = await new StreamReader(reqData.Body).ReadToEndAsync();
                var json = JsonConvert.DeserializeObject<JObject>(requestBody);

                if (json == null)
                {
                    json = new JObject();
                }

                string result = "";
                if (token == null)
                {
                    result = await RecommendationLogic.GetRecommendation(json, null);
                }
                else
                {
                    // Decode the token
                    Dictionary<string, string> tokenData = decoder.Decode(token);
                    _logger.LogError(decoder.DictionaryToString(tokenData));
                    result = await RecommendationLogic.GetRecommendation(json, tokenData);
                }

                // Convert the result to JSON
                var jsonResult = JsonConvert.SerializeObject(result);

                // Set the response status code and body
                response.StatusCode = HttpStatusCode.OK;
                response.Body = new MemoryStream(Encoding.UTF8.GetBytes(jsonResult));
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);

                // In case of an error, return an Internal Server Error
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.Body = new MemoryStream(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new { error = "An error occurred while getting the recommendations" })));
                return response;
            }
        }
    }
}
