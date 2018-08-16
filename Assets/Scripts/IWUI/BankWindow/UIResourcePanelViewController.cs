using UnityEngine;

public class UIResourcePanelViewController : UIGenericResourcePanelViewController 
{
    public void DebugCurrentResources()
    {
        CurrencyHellper.Purchase(itemUid, itemUid == Currency.Crystals.Name ? 5 : 100, null, new Vector2(Screen.width/2, Screen.height/2));
    }
}