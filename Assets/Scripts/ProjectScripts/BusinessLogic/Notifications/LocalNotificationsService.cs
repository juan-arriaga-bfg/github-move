public class LocalNotificationsService : IWService<LocalNotificationsService, LocalNotificationsManager> 
{
    public static LocalNotificationsManager Current
    {
        get { return Instance.Manager as LocalNotificationsManager; }
    }
}