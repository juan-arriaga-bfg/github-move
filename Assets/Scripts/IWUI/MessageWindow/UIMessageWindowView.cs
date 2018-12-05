using UnityEngine;
using UnityEngine.UI;

public class UIMessageWindowView : UIGenericPopupWindowView
{
    [IWUIBinding("#ButtonAcceptLabel")] protected NSText buttonAcceptLabel;
    [IWUIBinding("#ButtonBuyLabel")] protected NSText buttonBuyLabel;
    [IWUIBinding("#ButtonCancelLabel")] protected NSText buttonCancelLabel;
    
    [IWUIBinding("#Timer")] protected GameObject timer;
    [IWUIBinding("#TimerLabel")] protected NSText timerLabel;
    
    [IWUIBinding("#Image")] protected Image image;
    
    [IWUIBinding("#Buttons")] protected GameObject buttonsPanel;
    [IWUIBinding("#ButtonAccept")] protected GameObject btnAccept;
    [IWUIBinding("#ButtonBuy")] protected GameObject btnBuy;
    [IWUIBinding("#ButtonCancel")] protected GameObject btnCancel;

    [IWUIBinding("#DelimiterImageAndText")] protected GameObject delimiterImageAndText;
    [IWUIBinding("#DelimiterTextAndButtons")] protected GameObject delimiterTextAndButtons;
    
    private bool isAccept;
    private bool isCancel;
    
    public override void OnViewShow()
    {
        base.OnViewShow();
        
        var windowModel = Model as UIMessageWindowModel;
        
        image.gameObject.SetActive(!string.IsNullOrEmpty(windowModel.Image));
        message.gameObject.SetActive(string.IsNullOrEmpty(windowModel.Image));
        
        if (!string.IsNullOrEmpty(windowModel.Image))
        {
            image.sprite = IconService.Current.GetSpriteById(windowModel.Image);
        }
        
        isAccept = false;
        isCancel = false;
        
        SetTitle(windowModel.Title);
        SetMessage(windowModel.Message);
        
        buttonAcceptLabel.Text = buttonBuyLabel.Text = windowModel.AcceptLabel;
        buttonCancelLabel.Text = windowModel.CancelLabel;
        
        btnAccept.SetActive(windowModel.isBuy == false && windowModel.OnAccept != null);
        btnBuy.SetActive(windowModel.isBuy && windowModel.OnAccept != null);
        btnCancel.SetActive(windowModel.OnCancel != null);
        timer.SetActive(windowModel.Timer != null);

        buttonsPanel.SetActive(btnAccept.activeSelf || btnBuy.activeSelf || btnCancel.activeSelf);
        
        ToggleVisibleComponents(windowModel.VisibleComponents);
        
        if (windowModel.Timer == null) return;
        
        windowModel.Timer.OnExecute += UpdateTimer;
        windowModel.Timer.OnComplete += CompleteTimer;
    }

    public override void OnViewCloseCompleted()
    {   
        base.OnViewCloseCompleted();
        
        var windowModel = Model as UIMessageWindowModel;

        windowModel.Image = null;
        
        if(windowModel.Timer != null)
        {
            windowModel.Timer.OnExecute -= UpdateTimer;
            windowModel.Timer.OnComplete -= CompleteTimer;
            windowModel.Timer = null;
        }
        
        if (isAccept || windowModel.isHardAccept) windowModel.OnAccept?.Invoke();
        if (isCancel) windowModel.OnCancel?.Invoke();

        windowModel.isHardAccept = false;
        windowModel.isBuy = false;

        windowModel.VisibleComponents = UIMessageWindowModel.WindowComponents.Auto;
    }

    public void OnClickAccept()
    {
        isAccept = true;
    }
    
    public void OnClickCancel()
    {
        isCancel = true;
    }

    private void UpdateTimer()
    {
        var windowModel = Model as UIMessageWindowModel;
        var isFree = windowModel.Timer.IsFree();
        
        timerLabel.Text = windowModel.Timer.CompleteTime.GetTimeLeftText();
        buttonBuyLabel.Text = isFree ? LocalizationService.Get("common.button.free", "common.button.free") : windowModel.AcceptLabel + windowModel.Timer.GetPrise().ToStringIcon(false);
    }

    private void CompleteTimer()
    {
        Controller.CloseCurrentWindow();
    }

    private void ToggleVisibleComponents(UIMessageWindowModel.WindowComponents visibleComponents)
    {
        if (visibleComponents == UIMessageWindowModel.WindowComponents.Auto)
        {
            return;
        }
        
        image                   .gameObject.SetActive(visibleComponents.Has(UIMessageWindowModel.WindowComponents.Image));
        message                 .gameObject.SetActive(visibleComponents.Has(UIMessageWindowModel.WindowComponents.Message));
        btnAccept               .gameObject.SetActive(visibleComponents.Has(UIMessageWindowModel.WindowComponents.ButtonAccept));
        btnBuy                  .gameObject.SetActive(visibleComponents.Has(UIMessageWindowModel.WindowComponents.ButtonBuy));
        btnCancel               .gameObject.SetActive(visibleComponents.Has(UIMessageWindowModel.WindowComponents.ButtonCancel));
        timer                   .gameObject.SetActive(visibleComponents.Has(UIMessageWindowModel.WindowComponents.Timer));
        delimiterImageAndText   .gameObject.SetActive(visibleComponents.Has(UIMessageWindowModel.WindowComponents.ImageAndMessageDelimiter));
        delimiterTextAndButtons .gameObject.SetActive(visibleComponents.Has(UIMessageWindowModel.WindowComponents.MessageAndButtonsDelimiter));
        
        buttonsPanel.SetActive(visibleComponents.Has(UIMessageWindowModel.WindowComponents.ButtonAccept) 
                            || visibleComponents.Has(UIMessageWindowModel.WindowComponents.ButtonBuy) 
                            || visibleComponents.Has(UIMessageWindowModel.WindowComponents.ButtonCancel));
    }
}