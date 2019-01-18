public class InternetMonitorService : IWService<InternetMonitorService, IInternetMonitor>
{
    public static InternetMonitor Current => Instance.Manager as InternetMonitor;
}