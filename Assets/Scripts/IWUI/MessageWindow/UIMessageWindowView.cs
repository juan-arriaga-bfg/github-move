using UnityEngine;
using UnityEngine.UI;

public class UIMessageWindowView : UIGenericPopupWindowView
{
    [SerializeField] private NSText buttonAcceptLabel;
    [SerializeField] private NSText buttonBuyLabel;
    [SerializeField] private NSText buttonCancelLabel;
    [SerializeField] private NSText timerLabel;
    
    [SerializeField] private Image imageMessage;
    
    [SerializeField] private GameObject btnAccept;
    [SerializeField] private GameObject btnBuy;
    [SerializeField] private GameObject btnCancel;
    [SerializeField] private GameObject timer;
    
    private bool isAccept;
    private bool isCancel;
    
    public override void OnViewShow()
    {
        base.OnViewShow();
        
        var windowModel = Model as UIMessageWindowModel;
        
        imageMessage.gameObject.SetActive(!string.IsNullOrEmpty(windowModel.Image));
        message.gameObject.SetActive(string.IsNullOrEmpty(windowModel.Image));

        if (!string.IsNullOrEmpty(windowModel.Image))
        {
            imageMessage.sprite = IconService.Current.GetSpriteById(windowModel.Image);
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
        buttonBuyLabel.Text = isFree ? LocalizationService.Get("common.button.free", "Free") : windowModel.AcceptLabel + windowModel.Timer.GetPrise().ToStringIcon(false);
    }

    private void CompleteTimer()
    {
        Controller.CloseCurrentWindow();
    }
}