namespace Domain.Models;

public class Notification
{
    public User User { get; set; }
    public string Message { get; set; }
    public DateTime TimeToSend { get; set; }

    public Notification() {}
    
    public Notification(User user, string message, DateTime timeToSend)
    {
        User = user;
        Message = message;
        TimeToSend = timeToSend;
    }
}