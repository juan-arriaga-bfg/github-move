using Debug = IW.Logger;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BfgLocalNotificationsManagerBase : LocalNotificationsManagerBase
{
    public BfgLocalNotificationsManagerBase()
    {
        RegisterNotifier(new Notifier(new NowTimer(), NotifyType.ComeBackToGame));
    }

    
#if UNITY_EDITOR
    
    protected override void CancelAllOnDevice()
    {
        //Nothing todo here
    }

    protected override void ScheduleAllOnDevice()
    {
        Print();
    }
    
#elif UNITY_ANDROID

    private int GetNotificationIconId()
    {
        try
        {
            using (var rsClass = new AndroidJavaClass("com.bigfishgames.bfgunityandroid.custom.ResourcesHelper"))
            {
                int result = rsClass.CallStatic<int>("GetNotificationIconId");
                return result;
            }
        }
        catch (Exception e)
        {
            Debug.LogError("[SherwoodLocalNotificationsManager] => GetNotificationIconId: Exception: " + e.Message);
            return -1;
        }
    }

    protected override void CancelAllOnDevice()
    {
        List<int> ids = new List<int>
        {
            NotifyType.DailyTimeout.Id,
            NotifyType.EnergyRefresh.Id,
            NotifyType.MarketRefresh.Id,
            NotifyType.MonumentBuild.Id,
            NotifyType.MonumentRefresh.Id,
            NotifyType.ComeBackToGame.Id,
        };
        
        foreach (var id in ids)
        {
            bfgLocalNotificationManager.cancelNotification(id); 
        }
    }

    protected override void ScheduleAllOnDevice()
    {
        int iconId = GetNotificationIconId();
        if (iconId < 0)
        {
            Debug.LogError("[SherwoodLocalNotificationsManager] => ScheduleAllOnDevice: Can't get icon. Notifications will not be scheduled");
            return;
        }
        
        Print();
        
        foreach (var item in notifyItems)
        {
            long unixTimestamp = UnixTimeHelper.DateTimeToUnixTimestamp(item.NotifyTime);
            long unixTimestampMilliseconds = unixTimestamp * 1000;
            bfgLocalNotificationManager.scheduleNotification(item.Title, item.Message, iconId, item.Id, unixTimestampMilliseconds, true);
        }
    }
    
#elif UNITY_IOS

    protected override void CancelAllOnDevice()
    {
        
    }

    protected override void ScheduleAllOnDevice()
    {

    }
    
#endif

}