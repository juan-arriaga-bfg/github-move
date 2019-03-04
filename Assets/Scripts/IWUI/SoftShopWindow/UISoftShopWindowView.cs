public class UISoftShopWindowView : UIShopWindowView
{
    public override void OnViewShow()
    {
        base.OnViewShow();
        
        var windowModel = Model as UIShopWindowModel;

        content.GetScrollRect().enabled = windowModel.Products.Count > 3;
    }
}