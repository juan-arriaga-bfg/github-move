using System;

public class NotificationItem
{
    private static int lastId = 0;
    private static int GetId()
    {
        return lastId++;
    }
    
    public int Id;
    public DateTime NotificationTime;
    public string Title;
    public string Message;

    public NotificationItem(string title, string message, DateTime notificationTime)
    {
        Id = GetId();
        Title = title;
        Message = message;
        NotificationTime = notificationTime;
    }
}