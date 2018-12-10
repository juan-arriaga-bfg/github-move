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
    
    [IWUIBinding("#ButtonAccept")] protected UIButtonViewController btnAccept;
    [IWUIBinding("#ButtonCancel")] protected UIButtonViewController btnCancel;
    [IWUIBinding("#ButtonBuy")] protected UIButtonViewController btnBuy;
    
    [IWUIBinding("#ButtonAcceptBack")] protected Image btnAcceptBack;
    [IWUIBinding("#ButtonCancelBack")] protected Image btnCancelBack;
    
    [IWUIBinding("#DelimiterImageAndText")] protected GameObject delimiterImageAndText;
    [IWUIBinding("#DelimiterTextAndButtons")] protected GameObject delimiterTextAndButtons;
    [IWUIBinding("#DelimiterTimerAndButtons")] protected GameObject delimiterTimerAndButtons;
    
    private bool isAccept;
    private bool isCancel;
    
    public override void InitView(IWWindowModel model, IWWindowController controller)
    {
        base.InitView(model, controller);

        btnAccept.Init()
            .ToState(GenericButtonState.Active)
            .OnClick(OnClickAccept);
        
        btnCancel.Init()
            .ToState(GenericButtonState.Active)
            .OnClick(OnClickCancel);
        
        btnBuy.Init()
            .ToState(GenericButtonState.Active)
            .OnClick(OnClickAccept);
    }
    
    public override void OnViewShow()
    {
        base.OnViewShow();
        
        var windowModel = Model as UIMessageWindowModel;
        
        image.gameObject.SetActive(!string.IsNullOrEmpty(windowModel.Image));
        messageLabel.gameObject.SetActive(!string.IsNullOrEmpty(windowModel.Message));
        
        if (!string.IsNullOrEmpty(windowModel.Image))
        {
            image.sprite = IconService.Current.GetSpriteById(windowModel.Image);
        }
        
        isAccept = false;
        isCancel = false;
        
        SetTitle(windowModel.Title);
        SetMessage(windowModel.Message);
        
        btnAcceptBack.sprite = IconService.Current.GetSpriteById($"button{windowModel.AcceptColor}");
        btnCancelBack.sprite = IconService.Current.GetSpriteById($"button{windowModel.CancelColor}");
        
        buttonAcceptLabel.Text = buttonBuyLabel.Text = windowModel.AcceptLabel;
        buttonCancelLabel.Text = windowModel.CancelLabel;
        
        btnCancel.CachedTransform.SetSiblingIndex(windowModel.IsAcceptLeft ? 2 : 0);
        
        btnAccept.gameObject.SetActive(windowModel.IsBuy == false && windowModel.OnAccept != null);
        btnBuy.gameObject.SetActive(windowModel.IsBuy && windowModel.OnAccept != null);
        btnCancel.gameObject.SetActive(windowModel.OnCancel != null);
        timer.SetActive(windowModel.Timer != null);
        delimiterTimerAndButtons.SetActive(windowModel.Timer != null);

        buttonsPanel.SetActive(btnAccept.gameObject.activeSelf || btnBuy.gameObject.activeSelf || btnCancel.gameObject.activeSelf);
        
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
        windowModel.ResetColor();
        
        if(windowModel.Timer != null)
        {
            windowModel.Timer.OnExecute -= UpdateTimer;
            windowModel.Timer.OnComplete -= CompleteTimer;
            windowModel.Timer = null;
        }
        
        if (isAccept || windowModel.IsHardAccept) windowModel.OnAccept?.Invoke();
        if (isCancel) windowModel.OnCancel?.Invoke();
        
        if (isAccept == false && isCancel == false && windowModel.IsHardAccept == false)
        {
            windowModel.OnClose?.Invoke();
            windowModel.OnClose = null;
        }
        
        windowModel.IsHardAccept = false;
        windowModel.IsBuy = false;
        windowModel.IsAcceptLeft = false;

        windowModel.VisibleComponents = UIMessageWindowModel.WindowComponents.Auto;
    }

    private void OnClickAccept()
    {
        Controller.CloseCurrentWindow();
        isAccept = true;
    }
    
    private void OnClickCancel()
    {
        Controller.CloseCurrentWindow();
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
        messageLabel            .gameObject.SetActive(visibleComponents.Has(UIMessageWindowModel.WindowComponents.Message));
        btnAccept               .gameObject.SetActive(visibleComponents.Has(UIMessageWindowModel.WindowComponents.ButtonAccept));
        btnBuy                  .gameObject.SetActive(visibleComponents.Has(UIMessageWindowModel.WindowComponents.ButtonBuy));
        btnCancel               .gameObject.SetActive(visibleComponents.Has(UIMessageWindowModel.WindowComponents.ButtonCancel));
        timer                   .gameObject.SetActive(visibleComponents.Has(UIMessageWindowModel.WindowComponents.Timer));
        delimiterImageAndText   .gameObject.SetActive(visibleComponents.Has(UIMessageWindowModel.WindowComponents.ImageAndMessageDelimiter));
        delimiterTextAndButtons .gameObject.SetActive(visibleComponents.Has(UIMessageWindowModel.WindowComponents.MessageAndButtonsDelimiter));
        delimiterTimerAndButtons.gameObject.SetActive(visibleComponents.Has(UIMessageWindowModel.WindowComponents.Timer));
        
        buttonsPanel.SetActive(visibleComponents.Has(UIMessageWindowModel.WindowComponents.ButtonAccept) 
                            || visibleComponents.Has(UIMessageWindowModel.WindowComponents.ButtonBuy) 
                            || visibleComponents.Has(UIMessageWindowModel.WindowComponents.ButtonCancel));
    }
}