using UnityEngine;

public class UIResourcePanelViewController : UIGenericResourcePanelViewController 
{
    public void DebugCurrentResources()
    {
        BoardService.Current.GetBoardById(0)?.BoardEvents.RaiseEvent(GameEventsCodes.ClosePieceUI, this);
        
        var product = new CurrencyPair{Currency = itemUid, Amount = itemUid == Currency.Crystals.Name ? 5 : 100};
        
        UIMessageWindowController.CreateDefaultMessage($"Do you want to cheat and get {product.ToStringIcon(false)} for free?", () =>
        {
            CurrencyHellper.Purchase(product, null, new Vector2(Screen.width/2, Screen.height/2));
        });
    }
}