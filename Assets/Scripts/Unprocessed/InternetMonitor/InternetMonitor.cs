using System;
using DG.Tweening;
using UnityEngine;

public class InternetMonitor : IInternetMonitor
{
    private InternetConnectionState state;

    private Func<bool> checkMethod;

    private const float CHECK_INTERVAL = 1f;

    public Action<InternetConnectionState> OnStateChange { get; set; }
    
    public IInternetMonitor SetNetworkCheckMethod(Func<bool> checkMethod)
    {
        DOTween.Kill(this);
        
        this.checkMethod = checkMethod;

        DOTween.Sequence()
               .SetId(this)
               .AppendCallback(Check)
               .AppendInterval(CHECK_INTERVAL)
               .SetLoops(int.MaxValue);
        
        return this;
    }

    private void Check()
    {
        bool isAvailable = checkMethod();

        switch (state)
        {
            case InternetConnectionState.Unknown:
                state = isAvailable ? InternetConnectionState.Available : InternetConnectionState.NotAvailable;
                break;
            
            case InternetConnectionState.Available:
                if (!isAvailable)
                {
                    Debug.LogFormat("InternetConnectionMonitor: State changed: {0} => {1}", state, InternetConnectionState.NotAvailable);

                    state = InternetConnectionState.NotAvailable;
                    SendEvent();
                }

                break;
            
            case InternetConnectionState.NotAvailable:
                if (isAvailable)
                {
                    Debug.LogFormat("InternetConnectionMonitor: State changed: {0} => {1}", state, InternetConnectionState.Available);

                    state = InternetConnectionState.Available;
                    SendEvent();
                }

                break;
        }
    }

    private void SendEvent()
    {
        OnStateChange?.Invoke(state);
    }
}