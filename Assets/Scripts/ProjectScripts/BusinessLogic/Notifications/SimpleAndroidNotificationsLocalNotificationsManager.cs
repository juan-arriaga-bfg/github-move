using Debug = IW.Logger;
using System;
using UnityEngine;

#if !UNITY_EDITOR
using Assets.SimpleAndroidNotifications;
using Assets.SimpleAndroidNotifications.Data;
using Assets.SimpleAndroidNotifications.Enums;
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

        if (notifyItems == null || notifyItems.Count == 0)
        {
            Debug.LogWarning($"[SimpleAndroidNotificationsLocalNotificationsManager] => ScheduleAllOnDevice: notifyItems == null || notifyItems.Count == 0");
        }
        
        foreach (var item in notifyItems)
        {
            var delay = item.NotifyTime - DateTime.UtcNow;
#if !UNITY_EDITOR
            var notificationParams = new NotificationParams
            {
                Id = item.Id,
                Delay = delay,
                Title = item.Title,
                Message = item.Message,
                LargeIcon = NotificationIcon.ic_notification_large.ToString(),
                SmallIcon = NotificationIcon.ic_notification_small
            };
            
            NotificationManager.SendCustom(notificationParams);
#endif
        } 
    }
}