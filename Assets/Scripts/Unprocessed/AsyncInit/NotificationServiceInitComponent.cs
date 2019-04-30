public class NotificationServiceInitComponent : AsyncInitComponentBase
{
    public override void Execute()
    {
        //init notificationmanager
        SimpleAndroidNotificationsLocalNotificationsManager notifyManager = new SimpleAndroidNotificationsLocalNotificationsManager();
        
        LocalNotificationsService.Instance.SetManager(notifyManager);

        isCompleted = true;
        OnComplete(this);
    }
}