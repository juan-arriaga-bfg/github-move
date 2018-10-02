using UnityEngine;
using UnityEngine.UI;

public class UIEnergyShopItem : IWUIWindowViewController
{
    [SerializeField] private Image icon;
    [SerializeField] private NSText product;
    [SerializeField] private NSText price;
    
    private ShopDef def;
    
    private bool isClick;
    
    public void Init(ShopDef def)
    {
        this.def = def;
        
        isClick = false;
        
        icon.sprite = IconService.Current.GetSpriteById(def.Icon);
        icon.SetNativeSize();
        
        product.Text = $"{Currency.Energy.Name}: +{def.Product.ToStringIcon(false)}";
        price.Text = $"Buy {def.Price.ToStringIcon(false)}";
    }

    public override void OnViewCloseCompleted()
    {
        if (isClick == false) return;
        
        CurrencyHellper.Purchase(def.Product, def.Price, null, new Vector2(Screen.width/2, Screen.height/2));
    }

    public void OnClick()
    {
        if(isClick) return;
		
        isClick = true;
        
        var board = BoardService.Current.GetBoardById(0);
        var position = board.BoardLogic.PositionsCache.GetRandomPositions(PieceType.Char1.Id, 1)[0];
        
        if(!board.BoardLogic.EmptyCellsFinder.CheckFreeSpaceNearPosition(position, 1))
        {
            isClick = false;
            UIErrorWindowController.AddError("Free space not found");
            return;
        }

        if (CurrencyHellper.IsCanPurchase(def.Price, true) == false)
        {
            isClick = false;
            return;
        }
        
        context.Controller.CloseCurrentWindow();
    }
}