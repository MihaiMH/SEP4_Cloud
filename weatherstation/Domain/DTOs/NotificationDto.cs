namespace weatherstation.Domain.DTOs
{
    public class NotificationDto
    {
        public int UserId { get; set; }
        public string Endpoint { get; set; }
        public string P256dh { get; set; }
        public string Auth { get; set; }
        public string Time { get; set; }

        public NotificationDto() { }

        public NotificationDto(int userId, string endpoint, string p256dh, string auth, string time)
        {
            UserId = userId;
            Endpoint = endpoint;
            P256dh = p256dh;
            Auth = auth;
            Time = time;
        }
    }
}
