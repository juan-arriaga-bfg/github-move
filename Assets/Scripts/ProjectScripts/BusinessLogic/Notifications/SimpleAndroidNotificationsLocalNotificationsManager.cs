#if SOME_NOT_EXISTING_DEFINE

using System;
using Assets.SimpleAndroidNotifications;
using Assets.SimpleAndroidNotifications.Data;

public class SimpleAndroidNotificationsLocalNotificationsManager : LocalNotificationsManagerBase
{
    protected override void CancelAllOnDevice()
    {
        NotificationManager.CancelAll();
    }

    protected override void ScheduleAllOnDevice()
    {
        Print();

        foreach (var item in notifyItems)
        {
            var delay = item.NotifyTime - DateTime.Now;
            var notificationParams = new NotificationParams
            {
                Id = item.Id,
                Delay = delay,
                Title = item.Title,
                Message = item.Message
            };
            
            NotificationManager.SendCustom(notificationParams);
        }
    }
}

#endif