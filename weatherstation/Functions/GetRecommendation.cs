using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Text;
using weatherstation.Application.LogicInterfaces;
using weatherstation.Utils;

namespace weatherstation.Functions
{
    public class GetRecommendation
    {
        private readonly ILogger<GetRecommendation> _logger;
        private IRecommendationLogic recommendationLogic;

        public GetRecommendation(ILogger<GetRecommendation> logger, IRecommendationLogic recommendationLogic)
        {
            _logger = logger;
            this.recommendationLogic = recommendationLogic;
        }

        [Function("GetRecommendation")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData reqData)
        {
            var response = reqData.CreateResponse();

            try
            {
                Token decoder = new Token();

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
                if (!decoder.IsTokenValid(token))
                {
                    result = await recommendationLogic.GetRecommendation(json, null);
                }
                else
                {
                    // Decode the token
                    Dictionary<string, string> tokenData = decoder.Decode(token);
                    _logger.LogError(decoder.DictionaryToString(tokenData));
                    result = await recommendationLogic.GetRecommendation(json, tokenData);
                }

                var jsonResult = JsonConvert.SerializeObject(new { recommendation = result }, Formatting.Indented);

                response.StatusCode = HttpStatusCode.OK;
                response.Body = new MemoryStream(Encoding.UTF8.GetBytes(jsonResult));
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);

                response.StatusCode = HttpStatusCode.InternalServerError;
                response.Body = new MemoryStream(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new { error = "An error occurred while getting the recommendations" }, Formatting.Indented)));
                return response;
            }
        }
    }
}
