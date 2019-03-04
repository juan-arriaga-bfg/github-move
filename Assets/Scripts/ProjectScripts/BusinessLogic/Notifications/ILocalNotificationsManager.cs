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
}