using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;
using weatherstation.Application.Logic;
using weatherstation.Utils;

namespace weatherstation.Functions
{
    public class AddNotification
    {
        private readonly ILogger<AddNotification> _logger;

        public AddNotification(ILogger<AddNotification> logger)
        {
            _logger = logger;
        }

        [Function("AddNotification")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData req)
        {
            var res = req.CreateResponse();

            try
            {
                Token decoder = new Token();
                string token = decoder.Extract(req);

                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                JObject json = JsonConvert.DeserializeObject<JObject>(requestBody);

                if (json == null)
                {
                    json = new JObject();
                }

                string result = "";
                if (decoder.IsTokenValid(token))
                {
                    res.StatusCode = System.Net.HttpStatusCode.Unauthorized;
                    var msg = JsonConvert.SerializeObject(new { msg = "Login in order to add notifications to account." }, Formatting.Indented);
                    res.Body = new MemoryStream(Encoding.UTF8.GetBytes(msg));
                    return res;
                }
                else
                {
                    Dictionary<string, string> tokenData = decoder.Decode(token);
                    NotificationLogic logic = new NotificationLogic();
                    await logic.AddNotificationAsync(json, tokenData);

                    res.StatusCode = System.Net.HttpStatusCode.OK;
                    res.Body = new MemoryStream(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new { msg = "You have added a notification to your account successfully." }, Formatting.Indented)));
                    return res;
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "An error occurred while adding notification.");
                res.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                res.Body = new MemoryStream(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new { error = "An error occurred while adding notification." }, Formatting.Indented)));
                return res;
            }
        }
    }
}
