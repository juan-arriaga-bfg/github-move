public class NotificationServiceInitComponent : AsyncInitComponentBase
{
    public override void Execute()
    {
        //init notificationmanager
        SimpleAndroidNotificationsLocalNotificationsManager notifyManager = new SimpleAndroidNotificationsLocalNotificationsManager();
        notifyManager.CancelNotifications();
        LocalNotificationsService.Instance.SetManager(notifyManager);

        isCompleted = true;
        OnComplete(this);
    }
}