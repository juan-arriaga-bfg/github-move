using UnityEngine;
using DG.Tweening;

public class UITimeSyncWindowView : UIGenericPopupWindowView 
{
    private enum WindowState
    {
        Default,
        Error,
        Connecting,
        Success
    }
    
    [IWUIBinding("#ImageError")] private Transform imageError;
    [IWUIBinding("#ImageSuccess")] private Transform imageSuccess;
    
    private SecuredTimeServiceInitComponent timeServiceInitComponent;

    private WindowState state;
    
    public override void OnViewShow()
    {
        base.OnViewShow();
        
        UITimeSyncWindowModel windowModel = Model as UITimeSyncWindowModel;

        state = WindowState.Default;
        
        imageError.localScale = Vector3.zero;
        imageSuccess.localScale = Vector3.zero;
        
        // Can't store in the model because windows cache may not exists at this time
        timeServiceInitComponent = AsyncInitService.Current.GetComponent<SecuredTimeServiceInitComponent>();
        
        timeServiceInitComponent.OnRetryScheduled += OnRetryScheduled;
        timeServiceInitComponent.OnRetry += OnRetry;
        timeServiceInitComponent.OnSuccess += OnSuccess;
    }

    protected override void PlayShowSound()
    {
    }
    
    protected override void PlayCloseSound()
    {
    }

    public override void OnViewShowCompleted()
    {
        base.OnViewShowCompleted();
        if(btnBackLayer != null) btnBackLayer.OnClick(() => { });
    }

    public override void OnViewClose()
    {
        base.OnViewClose();
        
        UITimeSyncWindowModel windowModel = Model as UITimeSyncWindowModel;

        timeServiceInitComponent.OnRetryScheduled -= OnRetryScheduled;
        timeServiceInitComponent.OnRetry -= OnRetry;
        timeServiceInitComponent.OnSuccess -= OnSuccess;
        
        DOTween.Kill(this);
        DOTween.Kill(imageError);
    }

    private void OnSuccess()
    {
        SetModeSuccess();
        
        DOTween.Kill(this);
        
        DOTween.Sequence().SetId(this)
            .AppendInterval(1)
            .AppendCallback(() => { Controller.CloseCurrentWindow(); });
    }

    private void OnRetry()
    {
        SetModeConnecting();
    }

    private void OnRetryScheduled(float delay)
    {
        int delayAsInt = (int) delay - 1;
        
       DOTween.Kill(this);
       
       var seq = DOTween.Sequence().SetId(this);
       
       for (int i = 0; i < delayAsInt; i++)
       {
           var index = i;
           
           seq.AppendInterval(1);
           seq.AppendCallback(() =>
           {
               SetModeError(delayAsInt - index);
           });
       }
    }

    private void SetModeError(int remainingTime)
    {
        UITimeSyncWindowModel windowModel = Model as UITimeSyncWindowModel;
        
        SetTitle(string.Format(windowModel.TitleError, remainingTime));
        SetMessage(windowModel.MessageError);
        
        if(state == WindowState.Error) return;

        state = WindowState.Error;
        
        DOTween.Kill(imageError, true);
        
        imageSuccess.DOScale(0, 0.1f).SetId(imageError);
        
        imageError.localScale = Vector3.zero;
            
        imageError.DOScale(1, 0.25f)
            .SetEase(Ease.OutBack)
            .OnComplete(() => imageError.transform.localScale = Vector3.one)
            .SetId(imageError);
    }
    
    private void SetModeConnecting()
    {
        UITimeSyncWindowModel windowModel = Model as UITimeSyncWindowModel;
        
        SetTitle(windowModel.TitleConnecting);
        SetMessage(windowModel.MessageConnecting);
        
        if(state == WindowState.Connecting) return;

        state = WindowState.Connecting;
        
        DOTween.Kill(imageError, true);
        
        imageError.DOScale(0, 0.1f).SetId(imageError);
        imageSuccess.DOScale(0, 0.1f).SetId(imageError);
    }
    
    private void SetModeSuccess()
    {
        UITimeSyncWindowModel windowModel = Model as UITimeSyncWindowModel;
        
        SetTitle(windowModel.TitleSuccess);
        SetMessage(windowModel.MessageSuccess);
        
        if(state == WindowState.Success) return;

        state = WindowState.Success;
        
        DOTween.Kill(imageError, true);
        
        imageError.DOScale(0, 0.1f).SetId(imageError);
        
        imageSuccess.localScale = Vector3.zero;
            
        imageSuccess.DOScale(1, 0.25f)
            .SetEase(Ease.OutBack)
            .OnComplete(() => imageSuccess.transform.localScale = Vector3.one)
            .SetId(imageError);
    }
}
