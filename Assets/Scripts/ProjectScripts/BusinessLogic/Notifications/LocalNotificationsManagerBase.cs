using Debug = IW.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class LocalNotificationsManagerBase : ILocalNotificationsManager
{
    private List<Notifier> notifiers = new List<Notifier>();
    protected List<Notification> notifyItems = new List<Notification>();

    public readonly TimeSpan NightEndTime   = new TimeSpan(8, 0, 0);
    public readonly TimeSpan MinimalTimeout = new TimeSpan(0, 2,0);

#if DEBUG
    private bool isDebugSchedule = false;
#endif

    public LocalNotificationsManagerBase()
    {
        RegisterNotifier(new Notifier(new NowTimer(), NotifyType.ComeBackToGame));
    }
    
    public void DebugSchedule()
    {
#if UNITY_EDITOR
        notifyItems.Clear();
        ScheduleNotifications();
        notifyItems.Clear();
#else
        notifyItems.Clear();
        CancelAllOnDevice();
        GenerateNotifications();

        TimeSpan timeShift = new TimeSpan(0, 0, 5);
        foreach (var notify in notifyItems)
        {
            notify.NotifyTime = DateTime.UtcNow + timeShift;
            timeShift = timeShift.Add(new TimeSpan(0, 0, 5));
        }
        
        ScheduleAllOnDevice();
        notifyItems.Clear();
#endif
        
#if DEBUG
        isDebugSchedule = true;
#endif
    }
    
    public void DebugScheduleAll()
    {
        var notifyTypes = NotifyType.NotifyTypes;
        notifyItems.Clear();
#if UNITY_EDITOR
        foreach (var notifyType in notifyTypes)
        {
            var notify = GenerateNotify(new Notifier(null, notifyType));
            notifyItems.Add(notify);
        }
        Print();
#else
        CancelAllOnDevice();
        TimeSpan timeShift = new TimeSpan(0, 1, 0);
        foreach (var notifyType in notifyTypes)
        {
            var notify = GenerateNotify(new Notifier(null, notifyType));
            notify.NotifyTime = DateTime.UtcNow + timeShift;
            timeShift = timeShift.Add(new TimeSpan(0, 0, 5));
            notifyItems.Add(notify);
        }
        ScheduleAllOnDevice();
#endif
        
        notifyItems.Clear();
        
#if DEBUG        
        isDebugSchedule = true;
#endif
    }
    
    public void RegisterNotifier(Notifier notifier)
    {
        if (notifier.Timer == null)
        {
            Debug.LogWarning($"[LocalNotificationService] => UnRegisterNotifier: timer == null");
            return;
        }

        if (notifiers.Any(currentNotifier => currentNotifier.Timer == notifier.Timer && currentNotifier.NotifyType == notifier.NotifyType))
        {
            Debug.LogWarning($"[LocalNotificationService] => notifier with id {notifier.NotifyType.Id} already registered with same timer");
            return;
        }
        
        Debug.Log($"[LocalNotificationService] => Register(NotifyType.Id:{notifier.NotifyType.Id})");
        notifiers.Add(notifier);
    }

    public void UnRegisterNotifier(Notifier notifier)
    {
        Debug.Log($"[LocalNotificationService] => UnRegisterNotifier(NotifyType.Id:{notifier.NotifyType.Id})");
        notifiers.Remove(notifier);
    }

    public void UnRegisterNotifier(TimerComponent timer)
    {
        if (timer == null)
        {
            Debug.LogWarning($"[LocalNotificationService] => UnRegisterNotifier: timer == null");
            return;
        }
        
        foreach (var notifier in notifiers)
        {
            if (notifier.Timer == timer)
            {
                Debug.Log($"[LocalNotificationService] => UnRegisterNotifier(NotifyType.Id:{notifier.NotifyType.Id})");
                notifiers.Remove(notifier);
                return;
            }
        }
    }

    public void PushNotify(Notification notification)
    {
        notifyItems.Add(notification);
    }

    public virtual void CancelNotifications()
    {
        Debug.Log("[LocalNotificationService] => CancelNotifications");

        notifyItems.Clear();
        
        CancelAllOnDevice();
        
#if DEBUG        
        isDebugSchedule = false;
#endif
    }

    public virtual void ScheduleNotifications()
    {
        Debug.Log($"[LocalNotificationService] => ScheduleNotifications (local time: {DateTime.Now}, UTC time: {DateTime.UtcNow})");

#if DEBUG
        if (isDebugSchedule)
        {
            Debug.Log($"[LocalNotificationService] => ScheduleNotification: Cancelled by isDebugSchedule == true");
            return;
        }
#endif
        
        GenerateNotifications();

        ScheduleAllOnDevice();
    }

    public virtual void GenerateNotifications()
    {
        var notifierTypes = notifiers.Select(notif => notif.NotifyType).Distinct();
        var notifications = new List<Notification>();
        
        foreach (var type in notifierTypes)
        {
            var selector = type.NotifySelector;
            var notifierList = this.notifiers.Where(elem => elem.NotifyType == type && elem.Timer.IsExecuteable()).ToList();
            if (selector != null)
            {
                notifierList = selector(notifierList);
            }
            
            foreach (var notifier in notifierList)
            {
                if (notifier.Timer.IsExecuteable())
                {
                    notifications.Add(GenerateNotify(notifier));
                }
            }
        }
        
        notifyItems.AddRange(notifications);
    }

    Notification GenerateNotify(Notifier notifier)
    {
        int id = notifier.NotifyType.Id;
        
        var titleKey = notifier.NotifyType.TitleKey;
        var messageKey = notifier.NotifyType.MessageKey;

        var title = LocalizationService.Current.GetTextByUid(titleKey, titleKey);
        var message = LocalizationService.Current.GetTextByUid(messageKey, messageKey);

        DateTime notifyDate = new DateTime(0);
        
        if (notifier.Timer != null)
        {
            notifyDate = DateTime.Now + notifier.Timer.CompleteTime.GetTimeLeft(notifier.Timer.UseUTC);
            if (notifier.NotifyType.TimeCorrector != null)
            {
                notifyDate = notifier.NotifyType.TimeCorrector(notifyDate);
            }
            notifyDate = CorrectTime(notifyDate);    
        }
        
        return new Notification(id, title, message, notifyDate);
    }

    public DateTime CorrectTime(DateTime notifyDate)
    {
        if (notifyDate - DateTime.Now < MinimalTimeout)
        {
            notifyDate = DateTime.Now + MinimalTimeout;
        }
            
        var time = new TimeSpan(notifyDate.Hour, notifyDate.Minute, notifyDate.Second);

        if(time < NightEndTime)
        {
            notifyDate = new DateTime(notifyDate.Year, notifyDate.Month, notifyDate.Day, NightEndTime.Hours, NightEndTime.Minutes, NightEndTime.Seconds);
        }

        notifyDate = notifyDate.ToUniversalTime();

        return notifyDate;
    }

    public void Print()
    {
        foreach (var item in notifyItems)
        {
            Debug.Log($"[LocalNotificationService] => Notification(Title: {item.Title}, Message: {item.Message}, NotifyTime: UTC {item.NotifyTime} | Local {item.NotifyTime.ToLocalTime()})");
        } 
    }

    public void Cleanup()
    {
        CancelNotifications();
        notifiers = null;
    }

    protected abstract void CancelAllOnDevice();

    protected abstract void ScheduleAllOnDevice();
}