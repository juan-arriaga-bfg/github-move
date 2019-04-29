public class UIEventWindowView : UIGenericPopupWindowView 
{
    [IWUIBinding("#MainTimerLabel")] private NSText timerLabel;
    [IWUIBinding("#ButtonShowLabel")] private NSText btnShowLabel;
    
    [IWUIBinding("#ButtonShow")] private UIButtonViewController btnShow;
    
    public override void OnViewShow()
    {
        base.OnViewShow();
        
        var windowModel = Model as UIEventWindowModel;
        
        SetTitle(windowModel.Title);
        SetMessage(windowModel.Message);

        btnShowLabel.Text = windowModel.ButtonText;
        
        windowModel.Timer.OnTimeChanged += OnTimeChanged;
        OnTimeChanged();
    }

    public override void OnViewShowCompleted()
    {
        base.OnViewShowCompleted();
        
        InitButtonBase(btnShow, OnShow);
    }
    
    public override void OnViewCloseCompleted()
    {
        var windowModel = Model as UIEventWindowModel;
        
        windowModel.Timer.OnTimeChanged -= OnTimeChanged;
        
        base.OnViewCloseCompleted();
    }

    private void OnShow()
    {
        
    }
    
    private void OnTimeChanged()
    {
        var windowModel = Model as UIEventPreviewWindowModel;

        timerLabel.Text = windowModel.TimerText;
    }
}
