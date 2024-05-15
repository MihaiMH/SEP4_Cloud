using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Text;
using weatherstation.Application.Logic;
using weatherstation.Utils;
using weatherstation.Domain.DTOs;

namespace weatherstation.Functions
{
    public class GetUserNotifications
    {
        private readonly ILogger<GetUserNotifications> _logger;

        public GetUserNotifications(ILogger<GetUserNotifications> logger)
        {
            _logger = logger;
        }

        [Function("GetUserNotifications")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequestData req)
        {
            var res = req.CreateResponse();

            try
            {
                TokenDecoder decoder = new TokenDecoder();
                string token = decoder.Extract(req);

                /*string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                JObject json = JsonConvert.DeserializeObject<JObject>(requestBody);

                if (json == null)
                {
                    json = new JObject();
                }*/

                if (token == null)
                {
                    res.StatusCode = System.Net.HttpStatusCode.Unauthorized;
                    var msg = JsonConvert.SerializeObject(new { msg = "Login in order to add notifications to account." });
                    res.Body = new MemoryStream(Encoding.UTF8.GetBytes(msg));
                    return res;
                }
                else
                {
                    Dictionary<string, string> tokenData = decoder.Decode(token);
                    List<NotificationDto> results = await NotificationLogic.GetNotificationsAsync(tokenData);

                    res.StatusCode = System.Net.HttpStatusCode.OK;
                    res.Body = new MemoryStream(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(results)));
                    return res;
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "An error occurred while adding notification.");
                res.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                res.Body = new MemoryStream(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new { error = "An error occurred while getting user's notifications." })));
                return res;
            }
        }
    }
}
