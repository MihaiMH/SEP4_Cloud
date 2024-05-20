using System.Text;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using weatherstation.Application.LogicInterfaces;
using weatherstation.Utils;

namespace weatherstation.Functions
{
    internal class UpdateAccount
    {
        private readonly ILogger<UpdateAccount> _logger;
        private IAccountLogic accountLogic;

        public UpdateAccount(ILogger<UpdateAccount> logger, IAccountLogic accountLogic)
        {
            _logger = logger;
            this.accountLogic = accountLogic;
        }

        [Function("UpdateAccount")]
        public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData reqData,
        FunctionContext executionContext)
        {
            var res = reqData.CreateResponse();

            try
            {
                Token decoder = new Token();
                string token = decoder.Extract(reqData);

                string requestBody = await new StreamReader(reqData.Body).ReadToEndAsync();
                JObject json = JsonConvert.DeserializeObject<JObject>(requestBody);

                if (json == null)
                {
                    json = new JObject();
                }

                string result = "";
                if (!decoder.IsTokenValid(token))
                {
                    res.StatusCode = System.Net.HttpStatusCode.Unauthorized;
                    var msg = JsonConvert.SerializeObject(new { msg = "Login in order to update account." }, Formatting.Indented);
                    res.Body = new MemoryStream(Encoding.UTF8.GetBytes(msg));
                    return res;
                }
                else
                {
                    Dictionary<string, string> tokenData = decoder.Decode(token);
                    result = await accountLogic.UpdateAccount(json, tokenData);

                    res.StatusCode = System.Net.HttpStatusCode.OK;
                    res.Body = new MemoryStream(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new { token = result }, Formatting.Indented)));
                    return res;
                }
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Error updating account: {Message}", ex.Message);
                res.StatusCode = System.Net.HttpStatusCode.BadRequest;
                res.Body = new MemoryStream(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new { error = ex.Message }, Formatting.Indented)));
                return res;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the account.");
                res.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                res.Body = new MemoryStream(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new { error = "An error occurred while updating the account." }, Formatting.Indented)));
                return res;
            }
        }

    }
}
