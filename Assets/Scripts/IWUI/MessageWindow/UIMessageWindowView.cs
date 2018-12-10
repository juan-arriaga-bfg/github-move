using System.Collections;
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
    [IWUIBinding("#Anchor")] protected RectTransform anchor;
    
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

    private Transform hint;
    
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
        
        if (!string.IsNullOrEmpty(windowModel.Image))
        {
            image.sprite = IconService.Current.GetSpriteById(windowModel.Image);
        }

        if (!string.IsNullOrEmpty(windowModel.Prefab))
        {
            hint = UIService.Get.PoolContainer.Create<Transform>((GameObject) ContentService.Current.GetObjectByName(windowModel.Prefab));
            hint.SetParentAndReset(anchor);
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
        
        image.gameObject.SetActive(!string.IsNullOrEmpty(windowModel.Image));
        messageLabel.gameObject.SetActive(!string.IsNullOrEmpty(windowModel.Message));
        anchor.gameObject.SetActive(!string.IsNullOrEmpty(windowModel.Prefab));
        
        btnAccept.gameObject.SetActive(windowModel.IsBuy == false && windowModel.OnAccept != null);
        btnBuy.gameObject.SetActive(windowModel.IsBuy && windowModel.OnAccept != null);
        btnCancel.gameObject.SetActive(windowModel.OnCancel != null);
        timer.SetActive(windowModel.Timer != null);
        
        delimiterImageAndText.SetActive(!string.IsNullOrEmpty(windowModel.Image) && !string.IsNullOrEmpty(windowModel.Message));
        delimiterTextAndButtons.SetActive(true);
        delimiterTimerAndButtons.SetActive(windowModel.Timer != null);

        buttonsPanel.SetActive(btnAccept.gameObject.activeSelf || btnBuy.gameObject.activeSelf || btnCancel.gameObject.activeSelf);
        
        StartCoroutine(UpdateLayoutCoroutine());
        
        if (windowModel.Timer == null) return;
        
        windowModel.Timer.OnExecute += UpdateTimer;
        windowModel.Timer.OnComplete += CompleteTimer;
    }
    
    private IEnumerator UpdateLayoutCoroutine()
    {
        yield return new WaitForEndOfFrame();
        LayoutRebuilder.ForceRebuildLayoutImmediate(anchor);
    }

    public override void OnViewCloseCompleted()
    {
        var windowModel = Model as UIMessageWindowModel;

        if (hint != null)
        {
            UIService.Get.PoolContainer.Return(hint.gameObject);
            hint = null;
        }
        
        if(windowModel.Timer != null)
        {
            windowModel.Timer.OnExecute -= UpdateTimer;
            windowModel.Timer.OnComplete -= CompleteTimer;
        }
        
        windowModel.Reset();
        
        if (isAccept || windowModel.IsHardAccept) windowModel.OnAccept?.Invoke();
        if (isCancel) windowModel.OnCancel?.Invoke();
        
        if (isAccept == false && isCancel == false && windowModel.IsHardAccept == false)
        {
            windowModel.OnClose?.Invoke();
        }
        
        windowModel.OnAccept = null;
        windowModel.OnCancel = null;
        windowModel.OnClose = null;
        
        base.OnViewCloseCompleted();
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
}