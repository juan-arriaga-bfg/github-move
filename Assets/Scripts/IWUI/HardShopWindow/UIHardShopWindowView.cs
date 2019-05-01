public class UIHardShopWindowView : UIShopWindowView 
{
    public override void OnViewShowCompleted()
    {
        base.OnViewShowCompleted();
        TackleBoxEvents.SendShopEnter();
    }

    public override void OnViewCloseCompleted()
    {
        base.OnViewCloseCompleted();
        TackleBoxEvents.SendShopClose();
    }
}