using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace weatherstation.Domain.Model
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
