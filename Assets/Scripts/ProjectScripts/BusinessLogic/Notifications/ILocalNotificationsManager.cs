using System;

public interface ILocalNotificationsManager
{
    void CancelNotifications();
    
    void ScheduleNotifications();

    void PushNotify(Notification notification);
    
    DateTime CorrectTime(DateTime notifyDate);

    void RegisterNotifier(Notifier notifier);

    void UnRegisterNotifier(TimerComponent timer);

    void Cleanup();
    
    // Schedule actual notifications - list is the same as if we are going to background right now
    // But with "fixed" timing
    void DebugSchedule();

    // Schedule ALL notifications, no matter have conditions met or not
    void DebugScheduleAll();
}