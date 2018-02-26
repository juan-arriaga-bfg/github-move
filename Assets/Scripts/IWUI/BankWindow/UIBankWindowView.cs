using System.Collections.Generic;
using UnityEngine;

public class UIBankWindowView : UIGenericPopupWindowView 
{
    public override void OnViewShow()
    {
        base.OnViewShow();
        
        UIBankWindowModel windowModel = Model as UIBankWindowModel;
        
    }

    public override void OnViewClose()
    {
        base.OnViewClose();
        
        UIBankWindowModel windowModel = Model as UIBankWindowModel;
        
    }

    public void PurchaseCoins()
    {
        var shopItem = new ShopItem
        {
            Uid = "purchase.test.coins.10", 
            ItemUid = Currency.Coins.Name, 
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
