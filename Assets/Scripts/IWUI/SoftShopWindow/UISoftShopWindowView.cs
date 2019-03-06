public class UISoftShopWindowView : UIShopWindowView
{
    public override void OnViewShow()
    {
        base.OnViewShow();
        
        var windowModel = Model as UIShopWindowModel;

        content.GetScrollRect().enabled = windowModel.Products.Count > 3;
    }

    public override void OnViewShowCompleted()
    {
        base.OnViewShowCompleted();
        TackleBoxEvents.SendCoinsOpen();
    }

    public override void OnViewCloseCompleted()
    {
        base.OnViewCloseCompleted();
        TackleBoxEvents.SendCoinsClosed();
    }
}