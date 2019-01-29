using System;

public interface ILocalNotificationsManager
{
    void CancelNotifications();
    
    void ScheduleNotifications();
    
    DateTime CorrectTime(DateTime notifyDate);

    void RegisterNotifier(Notifier notifier);

    void UnRegisterNotifier(TimerComponent timer);
}