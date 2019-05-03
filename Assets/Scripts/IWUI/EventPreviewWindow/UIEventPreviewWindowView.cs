public class UIEventPreviewWindowView : UIGenericPopupWindowView
{
    [IWUIBinding("#MainTimerLabel")] private NSText timerLabel;
    [IWUIBinding("#ButtonAcceptLabel")] private NSText btnAcceptLabel;
    
    [IWUIBinding("#ButtonAccept")] private UIButtonViewController btnAccept;
    
    public override void OnViewShow()
    {
        base.OnViewShow();
        
        var windowModel = Model as UIEventPreviewWindowModel;
        
        SetTitle(windowModel.Title);
        SetMessage(windowModel.Message);

        btnAcceptLabel.Text = windowModel.ButtonText;

        windowModel.Countdown.OnTimeChanged += OnTimeChanged;
        windowModel.Countdown.OnComplete += Controller.CloseCurrentWindow;
        OnTimeChanged();
    }

    public override void OnViewShowCompleted()
    {
        base.OnViewShowCompleted();
        
        InitButtonBase(btnAccept, Controller.CloseCurrentWindow);
    }

    public override void OnViewCloseCompleted()
    {
        var windowModel = Model as UIEventPreviewWindowModel;
        
        windowModel.Countdown.OnTimeChanged -= OnTimeChanged;
        windowModel.Countdown.OnComplete -= Controller.CloseCurrentWindow;
        
        base.OnViewCloseCompleted();
    }

    private void OnTimeChanged()
    {
        var windowModel = Model as UIEventPreviewWindowModel;

        timerLabel.Text = windowModel.TimerText;
    }
}
