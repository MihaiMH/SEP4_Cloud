using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using MySqlX.XDevAPI.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Ocsp;
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
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData reqData)
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
                JObject json = JsonConvert.DeserializeObject<JObject>(requestBody);

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
                // Return the token as the response
                return new OkObjectResult(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new OkObjectResult(ex);
            }
        }
    }
}
