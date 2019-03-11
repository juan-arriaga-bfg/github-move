using System;

#if !UNITY_EDITOR
using Assets.SimpleAndroidNotifications;
using Assets.SimpleAndroidNotifications.Data;
#endif

public class SimpleAndroidNotificationsLocalNotificationsManager : LocalNotificationsManagerBase
{
    protected override void CancelAllOnDevice()
    {
#if !UNITY_EDITOR
        NotificationManager.CancelAll();
#endif
    }

    protected override void ScheduleAllOnDevice()
    {
        Print();
        
#if !UNITY_EDITOR
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
#endif
        
    }
}