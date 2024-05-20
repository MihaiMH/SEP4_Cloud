using weatherstation.Application.LogicInterfaces;
using weatherstation.Domain.DTOs;
using weatherstation.Utils;

namespace weatherstation.Application.Logic
{
    public class NotificationLogic : INotificationLogic
    {
        private IAccountLogic accountLogic;
        private readonly IDBManager dbManager;
        
        public NotificationLogic(IAccountLogic accountLogic, IDBManager dbManager)
        {
            this.accountLogic = accountLogic;
            this.dbManager = dbManager;
        }

        public NotificationLogic() { 
        }

        public async Task<List<NotificationDto>> GetNotificationsAsync()
        {
            string? query = Environment.GetEnvironmentVariable("SQLCON1Q10", EnvironmentVariableTarget.Process);

            List<NotificationDto> results = await dbManager.ExecuteQuery(
                query,
                async (reader) => await Task.FromResult(new NotificationDto
                {
                    UserId = reader.GetInt32("Id"),
                    Endpoint = reader.GetString("Endpoint"),
                    P256dh = reader.GetString("P256dh"),
                    Auth = reader.GetString("Auth"),
                    Time = reader.GetString("Time")
                }));

            return results;
        }

        public async Task AddNotificationAsync(dynamic data, Dictionary<string, string> token)
        {
            string emailFromToken = token["email"];

            var existingUser = await accountLogic.GetUserByEmail(emailFromToken);

            string time = data["time"];
            string endpoint = data["endpoint"];
            string p256dh = data["p256dh"];
            string auth = data["auth"];

            string query = Environment.GetEnvironmentVariable("SQLCON1Q11", EnvironmentVariableTarget.Process);
            query = query
                .Replace("[VAR_TIME]", time)
                .Replace("[VAR_USERID]", existingUser.Id.ToString());

            await dbManager.InsertData(query);

            string newQuery = Environment.GetEnvironmentVariable("SQLCON1Q12", EnvironmentVariableTarget.Process);
            newQuery = newQuery
                .Replace("[VAR_AUTH]", auth)
                .Replace("[VAR_ENDPOINT]", endpoint)
                .Replace("[VAR_P256DH]", p256dh)
                .Replace("[VAR_USERID]", existingUser.Id.ToString());

            await dbManager.InsertData(newQuery);
        }

        public async Task<List<NotificationDto>> GetNotificationsAsync(Dictionary<string, string> token)
        {
            string emailFromToken = token["email"];

            var existingUser = await accountLogic.GetUserByEmail(emailFromToken);

            string query = Environment.GetEnvironmentVariable("SQLCON1Q13", EnvironmentVariableTarget.Process);
            query = query
                .Replace("[VAR_USERID]", existingUser.Id.ToString());

            List<NotificationDto> result = await dbManager.ExecuteQuery(
                query,
                async (reader) => await Task.FromResult(new NotificationDto
                {
                    UserId = reader.GetInt32("Id"),
                    Endpoint = reader.GetString("Endpoint"),
                    P256dh = reader.GetString("P256dh"),
                    Auth = reader.GetString("Auth"),
                    Time = reader.GetString("Time")
                }));

            return result;
        }
    }
}
