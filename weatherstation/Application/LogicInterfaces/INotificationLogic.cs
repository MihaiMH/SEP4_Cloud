using weatherstation.Domain.DTOs;

namespace weatherstation.Application.LogicInterfaces
{
    public interface INotificationLogic
    {
        Task<List<NotificationDto>> GetNotificationsAsync();
        Task AddNotificationAsync(dynamic data, Dictionary<string, string> token);
        Task<List<NotificationDto>> GetNotificationsAsync(Dictionary<string, string> token);
    }
}
