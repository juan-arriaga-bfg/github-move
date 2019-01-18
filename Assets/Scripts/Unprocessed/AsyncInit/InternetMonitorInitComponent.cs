public class InternetMonitorInitComponent : AsyncInitComponentBase
{
    public override void Execute()
    {
        InternetMonitor monitor = new InternetMonitor();
        InternetMonitorService.Instance.SetManager(monitor);
        monitor.SetNetworkCheckMethod(() => NetworkUtils.CheckInternetConnection());
        
        isCompleted = true;
        OnComplete(this);
    }
}