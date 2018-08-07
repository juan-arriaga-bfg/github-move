using UnityEngine;
using UnityEngine.UI;

public class UIEnergyShopItem : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private NSText product;
    [SerializeField] private NSText price;
    
    private ChestDef chest;
    
    private bool isClick;
    
    public void Init(ChestDef def)
    {
        chest = def;
        
        isClick = false;
        
        icon.sprite = IconService.Current.GetSpriteById(chest.Uid);
        icon.SetNativeSize();
        
        var resource = CurrencyHellper.ResourcePieceToCurrence(def.GetHardPieces(), Currency.Energy.Name);
        
        product.Text = string.Format("{0}: +{1}", Currency.Energy, resource.ToStringIcon(false));
        price.Text = string.Format("Buy {0}", def.Price.ToStringIcon(false));
    }

    public void OnClick()
    {
        if(isClick) return;
		
        isClick = true;
        
        var board = BoardService.Current.GetBoardById(0);
        if(!board.BoardLogic.EmptyCellsFinder.CheckFreeSpaceNearPosition(GameDataService.Current.PiecesManager.CastlePosition, 1))
        {
            UIErrorWindowController.AddError("Free space not found");
            return;
        }
        
        CurrencyHellper.Purchase(Currency.Chest.Name, 1, chest.Price, success =>
        {
            if(success == false) return;
			
            var model = UIService.Get.GetCachedModel<UIEnergyShopWindowModel>(UIWindowType.EnergyShopWindow);
            model.ChestReward = chest.Piece;
            UIService.Get.CloseWindow(UIWindowType.EnergyShopWindow, true);
        });
    }
}