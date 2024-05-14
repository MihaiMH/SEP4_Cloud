using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using weatherstation.Application.Logic;

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
            try
            {
                // Citim ce trimit ciuspanii din front
                string requestBody = await new StreamReader(reqData.Body).ReadToEndAsync();
                dynamic json = JsonConvert.DeserializeObject<dynamic>(requestBody);

                // Register the account
                string result = await RecommendationLogic.GetRecommendation(json);

                return new OkObjectResult(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);

                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
