using System.Collections.Generic;

public class LocalNotificationsManager
{
    private List<NotificationTimerItem> notificationTimers = new List<NotificationTimerItem>();
    
    public void RegisterNotificationTimer(TimerComponent timer, NotificationMessageInfo notificationMessage)
    {
        notificationTimers.Add(new NotificationTimerItem {Timer = timer, MessageInfo = notificationMessage});
    }

    public void UnRegisterNotificationTimer(TimerComponent timer)
    {
        for (int i = 0; i < notificationTimers.Count; i++)
        {
            var notificationItem = notificationTimers[i];
            if (notificationItem.Timer == timer)
            {
                notificationTimers.RemoveAt(i);
                return;
            }
        }
    }

    public List<NotificationItem> CreateNotifications()
    {
        var notifications = new List<NotificationItem>();
        foreach (var item in notificationTimers)
        {
            notifications.Add(CreateNotfy(item));
        }

        return notifications;
    }

    private NotificationItem CreateNotfy(NotificationTimerItem notifyTimer)
    {
        var title = LocalizationService.Current.GetTextByUid(notifyTimer.MessageInfo.TitleKey,
            notifyTimer.MessageInfo.TitleKey);
        var message = LocalizationService.Current.GetTextByUid(notifyTimer.MessageInfo.MessageKey,
            notifyTimer.MessageInfo.MessageKey);
        
        return new NotificationItem(title, message, notifyTimer.Timer.CompleteTime);
    }
}