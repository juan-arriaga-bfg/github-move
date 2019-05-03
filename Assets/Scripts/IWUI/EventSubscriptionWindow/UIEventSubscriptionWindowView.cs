public class UIEventSubscriptionWindowView : UIGenericPopupWindowView 
{
    [IWUIBinding("#ButtonAcceptLabel")] private NSText btnAcceptLabel;
    
    [IWUIBinding("#ButtonAccept")] private UIButtonViewController btnAccept;
    
    public override void OnViewShow()
    {
        base.OnViewShow();
        
        var windowModel = Model as UIEventSubscriptionWindowModel;
        
        SetTitle(windowModel.Title);
        SetMessage(windowModel.Message);

        btnAcceptLabel.Text = windowModel.ButtonText;
    }
    
    public override void OnViewShowCompleted()
    {
        base.OnViewShowCompleted();
        
        InitButtonBase(btnAccept, Controller.CloseCurrentWindow);
    }
}
