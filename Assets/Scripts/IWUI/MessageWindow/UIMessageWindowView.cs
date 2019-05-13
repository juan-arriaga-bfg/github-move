using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIMessageWindowView : UIGenericPopupWindowView
{
    [IWUIBinding("#MessageTop")] protected NSText messageTop;
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
    
    [IWUIBinding("#DelimiterImageAndTextTop")] protected GameObject delimiterImageAndTextTop;
    [IWUIBinding("#DelimiterImageAndText")] protected GameObject delimiterImageAndText;
    [IWUIBinding("#DelimiterTextAndButtons")] protected GameObject delimiterTextAndButtons;
    [IWUIBinding("#DelimiterTimerAndButtons")] protected GameObject delimiterTimerAndButtons;
    
    [IWUIBinding("#ButtonCancel")] protected CanvasGroup btnCancelCanvas;
    [IWUIBinding("#Shine")] protected RectTransform shine;
    
    private bool isAccept;
    private bool isCancel;

    private Transform hint;
    
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
        
        btnAccept.GetComponent<RectTransform>().sizeDelta = Vector2.one * windowModel.ButtonSize;
        btnCancel.GetComponent<RectTransform>().sizeDelta = Vector2.one * windowModel.ButtonSize;
        btnBuy.GetComponent<RectTransform>().sizeDelta = Vector2.one * windowModel.ButtonSize;
        
        SetTitle(windowModel.Title);
        SetMessage(windowModel.Message);

        messageTop.Text = windowModel.Message;
        
        btnAcceptBack.sprite = IconService.Current.GetSpriteById($"button{windowModel.AcceptColor}");
        btnCancelBack.sprite = IconService.Current.GetSpriteById($"button{windowModel.CancelColor}");
        
        buttonAcceptLabel.Text = buttonBuyLabel.Text = windowModel.AcceptLabel;
        buttonCancelLabel.Text = windowModel.CancelLabel;
        
        btnCancel.CachedTransform.SetSiblingIndex(windowModel.IsAcceptLeft ? 2 : 0);
        
        image.gameObject.SetActive(!string.IsNullOrEmpty(windowModel.Image));
        message.gameObject.SetActive(!windowModel.IsTopMessage && !string.IsNullOrEmpty(windowModel.Message));
        messageTop.gameObject.SetActive(windowModel.IsTopMessage);
        anchor.gameObject.SetActive(!string.IsNullOrEmpty(windowModel.Prefab));
        
        btnAccept.gameObject.SetActive(windowModel.IsBuy == false && windowModel.OnAccept != null);
        btnBuy.gameObject.SetActive(windowModel.IsBuy && windowModel.OnAccept != null);
        btnCancel.gameObject.SetActive(windowModel.OnCancel != null);
        timer.SetActive(windowModel.Timer != null);
        shine.gameObject.SetActive(windowModel.IsShine);
        
        shine.sizeDelta = Vector2.one * windowModel.ShineSize;
        
        delimiterImageAndTextTop.SetActive(windowModel.IsTopMessage);
        delimiterImageAndText.SetActive(windowModel.IsTopMessage == false && (!string.IsNullOrEmpty(windowModel.Image) || !string.IsNullOrEmpty(windowModel.Prefab)) && !string.IsNullOrEmpty(windowModel.Message));
        delimiterTextAndButtons.SetActive(true);
        delimiterTimerAndButtons.SetActive(windowModel.Timer != null);

        buttonsPanel.SetActive(btnAccept.gameObject.activeSelf || btnBuy.gameObject.activeSelf || btnCancel.gameObject.activeSelf);
        
        StartCoroutine(UpdateLayoutCoroutine());

        Controller.Window.IgnoreBackButton = windowModel.ProhibitClose;
        btnBackLayer.enabled = !windowModel.ProhibitClose;
        btnClose.gameObject.SetActive(!windowModel.ProhibitClose);

        if (windowModel.Timer == null) return;
        
        windowModel.Timer.OnTimeChanged += UpdateTimer;
        windowModel.Timer.OnComplete += CompleteTimer;
        UpdateTimer();
    }

    public override void OnViewShowCompleted()
    {
        base.OnViewShowCompleted();
        
        InitButtonBase(btnAccept, OnClickAccept);
        InitButtonBase(btnCancel, OnClickCancel);
        InitButtonBase(btnBuy, OnClickAccept);
        
        shine.localPosition = new Vector3(0, anchor.localPosition.y);
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
            windowModel.Timer.OnTimeChanged -= UpdateTimer;
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
        var windowModel = Model as UIMessageWindowModel;
        if (windowModel.ProhibitClose)
        {
            windowModel.OnAccept?.Invoke();
            return;
        }
        
        Controller.CloseCurrentWindow();
        isAccept = true;
    }
    
    private void OnClickCancel()
    {
        var windowModel = Model as UIMessageWindowModel;
        if (windowModel.ProhibitClose)
        {
            windowModel.OnCancel?.Invoke();
            return;
        }
        
        Controller.CloseCurrentWindow();
        isCancel = true;
    }

    private void UpdateTimer()
    {
        var windowModel = Model as UIMessageWindowModel;
        var isFree = windowModel.Timer.IsFree();
        
        timerLabel.Text = windowModel.Timer.CompleteTime.GetTimeLeftText();
        buttonBuyLabel.Text = isFree ? LocalizationService.Get("common.button.free", "common.button.free") : windowModel.AcceptLabel + windowModel.Timer.GetPrice().ToStringIcon();
        btnCancelBack.sprite = IconService.Current.GetSpriteById($"button{(isFree ? UIMessageWindowModel.ButtonColor.Sepia : windowModel.CancelColor)}");
        btnCancelCanvas.interactable = !isFree;
    }

    private void CompleteTimer()
    {
        var windowModel = Model as UIMessageWindowModel;
        if (!windowModel.ProhibitClose)
        {
            Controller.CloseCurrentWindow();
        }
    }
}