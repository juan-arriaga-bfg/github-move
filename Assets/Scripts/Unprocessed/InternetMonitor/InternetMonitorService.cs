public class InternetMonitorService : IWService<InternetMonitorService, IInternetMonitor>
{
    public static IInternetMonitor Current => Instance.Manager as IInternetMonitor;
}