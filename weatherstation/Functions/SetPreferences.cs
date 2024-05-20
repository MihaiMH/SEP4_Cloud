using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Org.BouncyCastle.Ocsp;
using System.Text;
using weatherstation.Application.Logic;
using weatherstation.Utils;
using weatherstation.Application.LogicInterfaces;

namespace weatherstation.Functions
{
    public class SetPreferences
    {
        private readonly ILogger<SetPreferences> _logger;
        private IAccountLogic accountLogic;

        public SetPreferences(ILogger<SetPreferences> logger, IAccountLogic accountLogic)
        {
            _logger = logger;
            this.accountLogic = accountLogic;
        }

        [Function("SetPreferences")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData req)
        {
            var res = req.CreateResponse();

            try
            {
                TokenDecoder decoder = new TokenDecoder();
                string token = decoder.Extract(req);

                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                JObject json = JsonConvert.DeserializeObject<JObject>(requestBody);

                if (json == null)
                {
                    json = new JObject();
                }

                string result = "";
                if (token == null)
                {
                    res.StatusCode = System.Net.HttpStatusCode.Unauthorized;
                    var msg = JsonConvert.SerializeObject(new { msg = "Login in order to set preferences." }, Formatting.Indented);
                    res.Body = new MemoryStream(Encoding.UTF8.GetBytes(msg));
                    return res;
                }
                else
                {
                    Dictionary<string, string> tokenData = decoder.Decode(token);
                    await accountLogic.SetPreferences(json, tokenData);

                    res.StatusCode = System.Net.HttpStatusCode.OK;
                    res.Body = new MemoryStream(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new { msg = "Preferences setted successfully." }, Formatting.Indented)));
                    return res;
                }
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Error setting preferences: {Message}", ex.Message);
                res.StatusCode = System.Net.HttpStatusCode.BadRequest;
                res.Body = new MemoryStream(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new { error = ex.Message }, Formatting.Indented)));
                return res;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the account.");
                res.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                res.Body = new MemoryStream(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new { error = "An error occurred while setting preferences." }, Formatting.Indented)));
                return res;
            }
        }
    }
}
