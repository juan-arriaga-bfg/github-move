using System;
using DG.Tweening;
using UnityEngine;

public class SecuredTimeServiceInitComponent : AsyncInitComponentBase
{
    public const float RETRY_DELAY = 5f;

    private bool isWindowShowed;

    public Action OnRetry;
    public Action OnSuccess;
    public Action<float> OnRetryScheduled;

    public SecuredTimeManager SecuredTimeManager { get; private set; }
    
    public override void Execute()
    {
        SecuredTimeManager timeManager = new SecuredTimeManager();
        SecuredTimeService.Instance.SetManager(timeManager);

        var mainUrl = NetworkUtils.Instance.GetHostUrl();

        timeManager
           .AddServerTimeProvider(new BfgServerTimeProvider().SetUrl($"{mainUrl}/time"))
           .AddServerTimeProvider(new BfgServerTimeProvider().SetUrl("https://f2p.bigfishgames.com/RobinHood/time"))
           ;

        SecuredTimeManager = timeManager;
        
        if (timeManager.IsSyncedWithServer)
        {
            Done();
            return;
        }
        
        SyncWithServer(0);
    }

    private bool CanShowWindow()
    {
        return !isWindowShowed && AsyncInitService.Current != null && AsyncInitService.Current.IsCompleted<LocalizationInitComponent>();
    }
    
    private void SyncWithServer(float delay)
    {
        OnRetryScheduled?.Invoke(delay);
        
        var service = SecuredTimeService.Current;

        DOTween.Sequence()
               .AppendInterval(delay)
               .AppendCallback(() =>
                {
                    OnRetry?.Invoke();
                    
                    service.Init(isOk =>
                    {
                        if (isOk)
                        {
                            Done();
                        }
                        else
                        {
                            if (CanShowWindow())
                            {
                                ShowErrorWindow();
                            }
                            SyncWithServer(RETRY_DELAY);
                        }
                    }, true);
                });
    }

    private void ShowErrorWindow()
    {
        if (isWindowShowed)
        {
            return;
        }

        // Still can't show windows
        if (!AsyncInitService.Current.IsCompleted<LocalAssetBundlesCacheInitComponent>())
        {
            return;
        }
        
        isWindowShowed = true;
        
        UIService.Get.ShowWindowInstantly(UIWindowType.TimeSyncWindow);
    }

    private void Done()
    {
        OnSuccess?.Invoke();
        isCompleted = true;
        OnComplete(this);
    }
}