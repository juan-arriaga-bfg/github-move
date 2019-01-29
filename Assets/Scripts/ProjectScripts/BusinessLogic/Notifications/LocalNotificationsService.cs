public class LocalNotificationsService : IWService<LocalNotificationsService, ILocalNotificationsManager> 
{
    public static ILocalNotificationsManager Current => Instance.Manager;
}