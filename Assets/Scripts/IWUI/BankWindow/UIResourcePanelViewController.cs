public class UIResourcePanelViewController : UIGenericResourcePanelViewController 
{
    public void DebugCurrentResources()
    {
        CurrencyHellper.Purchase(itemUid, itemUid == Currency.Crystals.Name ? 5 : 100);
    }
}