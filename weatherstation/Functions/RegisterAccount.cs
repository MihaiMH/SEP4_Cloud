using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using weatherstation.Application.Logic;
using weatherstation.Domain.Model;
using System.Text;

namespace weatherstation.Functions
{
    public class RegisterAccount
    {
        private readonly ILogger<RegisterAccount> _logger;

        public RegisterAccount(ILogger<RegisterAccount> logger)
        {            _logger = logger;               }

        [Function("RegisterAccount")]
        public async Task<HttpResponseData> RunAsync([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData reqData)
        {
            var res = reqData.CreateResponse();

            try
            {
                string requestBody = await new StreamReader(reqData.Body).ReadToEndAsync();
                dynamic data = JsonConvert.DeserializeObject<dynamic>(requestBody);
                await AccountLogic.RegisterAccount(data);

                res.StatusCode = System.Net.HttpStatusCode.OK;
                res.Body = new MemoryStream(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new { msg = "You have successfully created an account." }, Formatting.Indented)));
                return res;
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Error registering: {Message}", ex.Message);
                res.StatusCode = System.Net.HttpStatusCode.BadRequest;
                res.Body = new MemoryStream(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new { error = ex.Message }, Formatting.Indented)));
                return res;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while registering.");
                res.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                res.Body = new MemoryStream(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new { error = "An error occurred while registering." }, Formatting.Indented)));
                return res;
            }
        }
    }
}
