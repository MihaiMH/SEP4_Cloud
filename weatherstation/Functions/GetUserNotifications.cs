using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Text;
using weatherstation.Utils;
using weatherstation.Domain.DTOs;
using weatherstation.Application.LogicInterfaces;

namespace weatherstation.Functions
{
    public class GetUserNotifications
    {
        private readonly ILogger<GetUserNotifications> _logger;
        private INotificationLogic notificationLogic;

        public GetUserNotifications(ILogger<GetUserNotifications> logger, INotificationLogic notificationLogic)
        {
            _logger = logger;
            this.notificationLogic = notificationLogic;
        }

        [Function("GetUserNotifications")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequestData req)
        {
            var res = req.CreateResponse();

            try
            {
                Token decoder = new Token();
                string token = decoder.Extract(req);

                if (!decoder.IsTokenValid(token))
                {
                    res.StatusCode = System.Net.HttpStatusCode.Unauthorized;
                    var msg = JsonConvert.SerializeObject(new { msg = "Login in order to add notifications to account." }, Formatting.Indented);
                    res.Body = new MemoryStream(Encoding.UTF8.GetBytes(msg));
                    return res;
                }
                else
                {
                    Dictionary<string, string> tokenData = decoder.Decode(token);
                    List<NotificationDto> results = await notificationLogic.GetNotificationsAsync(tokenData);

                    res.StatusCode = System.Net.HttpStatusCode.OK;
                    res.Body = new MemoryStream(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(results, Formatting.Indented)));
                    return res;
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "An error occurred while adding notification.");
                res.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                res.Body = new MemoryStream(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new { error = "An error occurred while getting user's notifications." }, Formatting.Indented)));
                return res;
            }
        }
    }
}
