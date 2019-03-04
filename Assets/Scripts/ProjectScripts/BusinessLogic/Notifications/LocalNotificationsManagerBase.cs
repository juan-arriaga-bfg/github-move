using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class LocalNotificationsManagerBase : ILocalNotificationsManager
{
    private List<Notifier> notifiers = new List<Notifier>();
    protected List<Notification> notifyItems = new List<Notification>();

    public readonly TimeSpan NightBeginTime = new TimeSpan(22, 0, 0);
    public readonly TimeSpan NightEndTime   = new TimeSpan(10, 0, 0);
    public readonly TimeSpan MinimalTimeout = new TimeSpan(0,  30,0);
    
    public void RegisterNotifier(Notifier notifier)
    {
        Debug.Log($"[LocalNotificationService] => Register(NotifyType.Id:{notifier.NotifyType.Id})");
        notifiers.Add(notifier);
    }

    public void UnRegisterNotifier(Notifier notifier)
    {
        Debug.Log($"[LocalNotificationService] => UnRegister(NotifyType.Id:{notifier.NotifyType.Id})");
        notifiers.Remove(notifier);
    }

    public void UnRegisterNotifier(TimerComponent timer)
    {
        foreach (var notifier in notifiers)
        {
            if (notifier.Timer == timer)
            {
                Debug.Log($"[LocalNotificationService] => UnRegister(NotifyType.Id:{notifier.NotifyType.Id})");
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
    }

    public virtual void ScheduleNotifications()
    {
        Debug.Log($"[LocalNotificationService] => ScheduleNotifications (CurrentTime: {DateTime.Now})");
        
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
            var notifiers = this.notifiers.Where(elem => elem.NotifyType == type && elem.Timer.IsExecuteable()).ToList();
            if (selector != null)
            {
                notifiers = selector(notifiers);
            }
            
            foreach (var notifier in notifiers)
            {
                if(notifier.Timer.IsExecuteable())
                    notifications.Add(GenerateNotify(notifier));
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
        
        var notifyDate = notifier.Timer.UseUTC ? notifier.Timer.CompleteTime.ToLocalTime() : notifier.Timer.CompleteTime;
        if (notifier.NotifyType.TimeCorrector != null)
            notifyDate = notifier.NotifyType.TimeCorrector(notifyDate);
        notifyDate = CorrectTime(notifyDate);
        return new Notification(id, title, message, notifyDate);
    }

    public DateTime CorrectTime(DateTime notifyDate)
    {
        if (notifyDate - DateTime.Now < MinimalTimeout)
        {
            notifyDate = DateTime.Now + MinimalTimeout;
        }
            
        var time = new TimeSpan(notifyDate.Hour, notifyDate.Minute, notifyDate.Second);
        if (time >= NightBeginTime)
        {
            notifyDate = notifyDate.AddDays(1);
            notifyDate = new DateTime(notifyDate.Year, notifyDate.Month, notifyDate.Day, NightEndTime.Hours, NightEndTime.Minutes, NightEndTime.Seconds);
        }
        else if(time < NightEndTime)
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
            Debug.Log($"[LocalNotificationService] => Notification(Title: {item.Title}, Message: {item.Message}, NotifyTime: UTC {item.NotifyTime})");
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