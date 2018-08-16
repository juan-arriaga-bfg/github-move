using UnityEngine;

public class UIResourcePanelViewController : UIGenericResourcePanelViewController 
{
    public void DebugCurrentResources()
    {
        BoardService.Current.GetBoardById(0)?.BoardEvents.RaiseEvent(GameEventsCodes.ClosePieceUI, this);
        CurrencyHellper.Purchase(itemUid, itemUid == Currency.Crystals.Name ? 5 : 100, null, new Vector2(Screen.width/2, Screen.height/2));
    }
}