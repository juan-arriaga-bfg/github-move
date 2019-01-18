using System;

public interface IInternetMonitor
{
    IInternetMonitor SetNetworkCheckMethod(Func<bool> checkMethod);
    Action<InternetConnectionState> OnStateChange { get; set; }

    bool IsInternetAvailable { get; }
}