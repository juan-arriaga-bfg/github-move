using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIResourcePanelViewController : UIGenericResourcePanelViewController 
{
    public void DebugAddCurrency()
    {
        var shopItem = new ShopItem
        {
            Uid = string.Format("purchase.test.{0}.10", itemUid), 
            ItemUid = itemUid, 
            Amount = 10,
            CurrentPrices = new List<Price>
            {
                new Price{Currency = Currency.Cash.Name, DefaultPriceAmount = 1}
            }
        };
        ShopService.Current.PurchaseItem
        (
            Currency.Coins.Name, 
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
