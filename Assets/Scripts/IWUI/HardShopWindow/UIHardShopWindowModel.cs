using System.Collections.Generic;

public class UIHardShopWindowModel : UIShopWindowModel 
{
    protected override CurrencyDef ShopType => Currency.Crystals;

    public override List<ShopDef> Products
    {
        get
        {
            var products = new List<ShopDef>(GameDataService.Current.ShopManager.Defs[ShopType.Name]);

            if (BoardService.Current.FirstBoard.MarketLogic.Offer != null) products.Insert(0, BoardService.Current.FirstBoard.MarketLogic.Offer);
            
            return products;
        }
    }
    
    public override string AnalyticLocation => $"shop_premium";
}