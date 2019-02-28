public class NotificationServiceInitComponent : AsyncInitComponentBase
{
    public override void Execute()
    {
        //init notificationmanager
        BfgLocalNotificationsManagerBase notifyManager = new BfgLocalNotificationsManagerBase();
        notifyManager.CancelNotifications();
        LocalNotificationsService.Instance.SetManager(notifyManager);

        isCompleted = true;
        OnComplete(this);
    }
}