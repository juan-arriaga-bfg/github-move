using System;

public class Notification
{
    public readonly int Id;
    public string Title;
    public string Message;
    public DateTime NotifyTime;

    public Notification(string title, string message, DateTime notifyTime)
    {
        Id = GetId();
        Title = title;
        Message = message;
        NotifyTime = notifyTime;
    }

    private static int nextId = 0;
    private static int GetId()
    {
        return nextId++;
    }
}