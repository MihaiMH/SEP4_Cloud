using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Azure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using weatherstation.Application.Logic;
using weatherstation.Utils;

namespace weatherstation.Functions
{
    internal class UpdateAccount
    {
        private readonly ILogger<UpdateAccount> _logger;

        public UpdateAccount(ILogger<UpdateAccount> logger)
        {
            _logger = logger;
        }

        [Function("UpdateAccount")]
        public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData reqData,
        FunctionContext executionContext)
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
                    var msg = JsonConvert.SerializeObject(new { msg = "Login in order to update account." });
                    res.Body = new MemoryStream(Encoding.UTF8.GetBytes(msg));
                    return res;
                }
                else
                {
                    Dictionary<string, string> tokenData = decoder.Decode(token);
                    result = await AccountLogic.UpdateAccount(json, tokenData);

                    res.StatusCode = System.Net.HttpStatusCode.OK;
                    res.Body = new MemoryStream(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new { token = result })));
                    return res;
                }
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Error updating account: {Message}", ex.Message);
                res.StatusCode = System.Net.HttpStatusCode.BadRequest;
                res.Body = new MemoryStream(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new { error = ex.Message })));
                return res;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the account.");
                res.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                res.Body = new MemoryStream(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new { error = "An error occurred while updating the account." })));
                return res;
            }
        }

    }
}
