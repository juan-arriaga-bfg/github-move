using System;

public class Notification
{
    public readonly int Id;
    public string Title;
    public string Message;
    public DateTime NotifyTime;

    public Notification(int id, string title, string message, DateTime notifyTime)
    {
        // Id = GetId();
        Id = id;
        Title = title;
        Message = message;
        NotifyTime = notifyTime;
    }
    //
    // private static int nextId = 0;
    // private static int GetId()
    // {
    //     return nextId++;
    // }
}