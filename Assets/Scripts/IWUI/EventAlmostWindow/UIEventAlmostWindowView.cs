public class UIEventAlmostWindowView : UIGenericPopupWindowView 
{
    [IWUIBinding("#Message2")] private NSText message2;
    [IWUIBinding("#Mark")] private NSText markLabel;
    [IWUIBinding("#ButtonBuyLabel")] private NSText btnBuyLabel;
    
    [IWUIBinding("#ButtonBuy")] private UIButtonViewController btnBuy;
    
    public override void OnViewShow()
    {
        base.OnViewShow();
        
        var windowModel = Model as UIEventAlmostWindowModel;
        
        SetTitle(windowModel.Title);
        SetMessage(windowModel.Message);
        
        message2.Text = windowModel.Message2;
        markLabel.Text = windowModel.MarkText;
        btnBuyLabel.Text = windowModel.ButtonText;
    }

    public override void OnViewShowCompleted()
    {
        base.OnViewShowCompleted();
        
        InitButtonBase(btnBuy, OnClick);
    }

    private void OnClick()
    {
        
    }
}
