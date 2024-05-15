using System;

namespace WEBSockets.Domain.Models
{
    public class Notification
    {
        public int Id { get; set; }
        public DateTime Time { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }

        public Notification() { }

        public Notification(int userId, DateTime time)
        {
            UserId = userId;
            Time = time;
        }
    }
}

