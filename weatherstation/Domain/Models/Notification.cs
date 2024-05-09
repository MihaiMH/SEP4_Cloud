using System;

public class Notification
{
    public User user { get; set; }
    public string Message { get; set; }
    public DateTime TimeToSend { get; set; }

    public Notification(User user, string message, DateTime timeToSend)
    {
        User = user;
        Message = message;
        TimeToSend = timeToSend;
    }
}
