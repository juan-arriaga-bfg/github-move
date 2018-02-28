using System.Collections.Generic;

public class UIResourcePanelViewController : UIGenericResourcePanelViewController 
{
    public void DebugAddCoins()
    {
        var shopItem = new ShopItem
        {
            Uid = string.Format("purchase.test.{0}.10", itemUid), 
            ItemUid = itemUid, 
            Amount = 100,
            CurrentPrices = new List<Price>
            {
                new Price{Currency = Currency.Cash.Name, DefaultPriceAmount = 0}
            }
        };
        
        ShopService.Current.PurchaseItem
        (
            shopItem,
            (item, s) =>
            {
                // on purchase success

            },
            item =>
            {
                // on purchase failed (not enough cash)
            }
        );
    }
    
    public void DebugAddCrystals()
    {
        var shopItem = new ShopItem
        {
            Uid = string.Format("purchase.test.{0}.10", itemUid), 
            ItemUid = itemUid, 
            Amount = 100,
            CurrentPrices = new List<Price>
            {
                new Price{Currency = Currency.Cash.Name, DefaultPriceAmount = 0}
            }
        };
        
        ShopService.Current.PurchaseItem
        (
            shopItem,
            (item, s) =>
            {
                // on purchase success

            },
            item =>
            {
                // on purchase failed (not enough cash)
            }
        );
    }
}
