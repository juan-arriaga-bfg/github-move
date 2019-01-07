using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEngine.UI;

public class UITimeSyncWindowView : UIGenericPopupWindowView 
{
    [IWUIBinding("#ImageSpinner")] protected Image imageSpinner;
    [IWUIBinding("#ImageError")] protected GameObject imageError;
    [IWUIBinding("#ImageSuccess")] protected GameObject imageSuccess;

    private SecuredTimeServiceInitComponent timeServiceInitComponent;
    
    public override void OnViewShow()
    {
        base.OnViewShow();
        
        UITimeSyncWindowModel windowModel = Model as UITimeSyncWindowModel;
        
        SetTitle(windowModel.Title);

        // Can't store in the model because windows cache may not exists at this time
        timeServiceInitComponent = AsyncInitService.Current.GetComponent<SecuredTimeServiceInitComponent>();
        
        timeServiceInitComponent.OnRetryScheduled += OnRetryScheduled;
        timeServiceInitComponent.OnRetry += OnRetry;
        timeServiceInitComponent.OnSuccess += OnSuccess;
    }

    public override void OnViewClose()
    {
        base.OnViewClose();
        
        UITimeSyncWindowModel windowModel = Model as UITimeSyncWindowModel;

        timeServiceInitComponent.OnRetryScheduled -= OnRetryScheduled;
        timeServiceInitComponent.OnRetry -= OnRetry;
        timeServiceInitComponent.OnSuccess -= OnSuccess;
        
        DOTween.Kill(this);
    }

    private void OnSuccess()
    {
        SetModeSuccess();
        Controller.CloseCurrentWindow();
    }

    private void OnRetry()
    {
        SetModeConnecting();
    }

    private void OnRetryScheduled(float delay)
    {
        int delayAsInt = (int) delay;
        
       SetModeError(delayAsInt);

       DOTween.Kill(this);
       
       var seq = DOTween.Sequence()
                        .SetId(this);
       
       for (int i = 1; i < delayAsInt; i++)
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
        
        imageSpinner.gameObject.SetActive(false);
        imageSuccess.gameObject.SetActive(false);
        imageError.gameObject.SetActive(true);
        message.Text = string.Format(windowModel.ErrorText, remainingTime);
    }
    
    private void SetModeConnecting()
    {
        UITimeSyncWindowModel windowModel = Model as UITimeSyncWindowModel;
        
        imageSpinner.gameObject.SetActive(true);
        imageError.gameObject.SetActive(false);
        imageSuccess.gameObject.SetActive(false);
        message.Text = windowModel.ConnectingText;
    }
    
    private void SetModeSuccess()
    {
        UITimeSyncWindowModel windowModel = Model as UITimeSyncWindowModel;
        
        imageSpinner.gameObject.SetActive(false);
        imageSuccess.gameObject.SetActive(true);
        imageError.gameObject.SetActive(false);
        message.Text = windowModel.SuccessText;
    }
}
