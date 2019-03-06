public class UIEnergyShopWindowView : UIShopWindowView 
{
    public override void OnViewShowCompleted()
    {
        base.OnViewShowCompleted();
        TackleBoxEvents.SendEnergyOpen();
    }

    public override void OnViewCloseCompleted()
    {
        base.OnViewCloseCompleted();
        TackleBoxEvents.SendEnergyClosed();
    }
}