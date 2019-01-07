using DG.Tweening;
using UnityEngine;

public class SecuredTimeServiceInitComponent : AsyncInitComponentBase
{
    public const float RETRY_DELAY = 5f;
    
    public override void Execute()
    {
        SecuredTimeManager timeManager = new SecuredTimeManager();
        SecuredTimeService.Instance.SetManager(timeManager);

        timeManager
           .AddServerTimeProvider(new BfgServerTimeProvider().SetUrl("https://f2p-qa.bigfishgames.com/RobinHood1/time"))
           .AddServerTimeProvider(new BfgServerTimeProvider().SetUrl("https://f2p.bigfishgames.com/RobinHood/time"));

        if (timeManager.IsSyncedWithServer)
        {
            Done();
            return;
        }
        
        SyncWithServer(0);
    }

    private void SyncWithServer(float delay)
    {
        var service = SecuredTimeService.Current;

        DOTween.Sequence()
               .AppendInterval(delay)
               .AppendCallback(() =>
                {
                    service.Init(isOk =>
                    {
                        if (isOk)
                        {
                            Done();
                        }
                        else
                        {
                            SyncWithServer(RETRY_DELAY);
                        }
                    }, true);
                });
    }

    private void Done()
    {
        isCompleted = true;
        OnComplete(this);
    }
}