using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Text;
using weatherstation.Application.LogicInterfaces;
using weatherstation.Domain.DTOs;

namespace weatherstation.Functions
{
    public class GetAllNotifications
    {
        private readonly ILogger<GetAllNotifications> _logger;
        private INotificationLogic notificationLogic;

        public GetAllNotifications(ILogger<GetAllNotifications> logger, INotificationLogic notificationLogic)
        {
            _logger = logger;
            this.notificationLogic = notificationLogic;
        }

        [Function("GetAllNotifications")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequestData req)
        {
            var res = req.CreateResponse();

            try
            {
                List<NotificationDto> dto = await notificationLogic.GetNotificationsAsync();

                res.StatusCode = System.Net.HttpStatusCode.OK;
                res.Body = new MemoryStream(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(dto, Formatting.Indented)));
                return res;
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "An error occurred while getting all notifications.");
                res.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                res.Body = new MemoryStream(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new { error = "An error occurred while getting all notifications." }, Formatting.Indented)));
                return res;
            }
        }
    }
}
