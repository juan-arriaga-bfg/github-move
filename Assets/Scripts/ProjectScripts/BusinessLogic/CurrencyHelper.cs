using System;
using System.Collections.Generic;
using System.Linq;

public static class CurrencyHellper
{
    public static bool Purchase(string product, int amount, Action<bool> onSuccess = null)
    {
        return Purchase(product, amount, Currency.Cash.Name, 0, onSuccess);
    }
    
    public static bool Purchase(string product, int amount, CurrencyPair price, Action<bool> onSuccess = null)
    {
        return Purchase(product, amount, price.Currency, price.Amount, onSuccess);
    }
    
    public static bool Purchase(CurrencyPair product, Action<bool> onSuccess = null)
    {
        return Purchase(product, new CurrencyPair{Currency = Currency.Cash.Name, Amount = 0}, onSuccess);
    }
    
    public static bool Purchase(CurrencyPair product, CurrencyPair price, Action<bool> onSuccess = null)
    {
        return Purchase(product.Currency, product.Amount, price.Currency, price.Amount, onSuccess);
    }
    
    public static bool Purchase(string product, int amountProduct, string price, int amountPrice, Action<bool> onSuccess = null)
    {
        var isSuccess = false;
        
        var shopItem = new ShopItem
        {
            Uid = string.Format("purchase.test.{0}.10", product), 
            ItemUid = product, 
            Amount = amountProduct,
            CurrentPrices = new List<Price>{new Price{Currency = price, DefaultPriceAmount = amountPrice}}
        };
        
        ShopService.Current.PurchaseItem
        (
            shopItem,
            (item, s) =>
            {
                // on purchase success
                isSuccess = true;
                if (onSuccess != null) onSuccess(true);
            },
            item =>
            {
                // on purchase failed (not enough cash)
                if (onSuccess != null) onSuccess(false);
                if(price == Currency.Coins.Name) UIMessageWindowController.CreateNeedCoinsMessage();
                else UIMessageWindowController.CreateNeedCurrencyMessage(price);
            }
        );

        return isSuccess;
    }

    public static bool Purchase(CurrencyPair product, List<CurrencyPair> prices, Action<bool> onSuccess = null)
    {
        return Purchase(product.Currency, product.Amount, prices, onSuccess);
    }
    
    public static bool Purchase(string product, int amount, List<CurrencyPair> prices, Action<bool> onSuccess = null)
    {
        var isSuccess = false;
        var currentPrices = new List<Price>();

        foreach (var price in prices)
        {
            currentPrices.Add(new Price{Currency = price.Currency, DefaultPriceAmount = price.Amount});
        }
        
        var shopItem = new ShopItem
        {
            Uid = string.Format("purchase.test.{0}.10", product), 
            ItemUid = product, 
            Amount = amount,
            CurrentPrices = currentPrices
        };
        
        ShopService.Current.PurchaseItem
        (
            shopItem,
            (item, s) =>
            {
                // on purchase success
                isSuccess = true;
                if (onSuccess != null) onSuccess(true);
            },
            item =>
            {
                // on purchase failed (not enough cash)
                if (onSuccess != null) onSuccess(false);
            }
        );
        
        return isSuccess;
    }

    public static bool IsCanPurchase(string price, int amount)
    {
        return ShopService.Current.IsCanPurchase(new Price{Currency = price, DefaultPriceAmount = amount});
    }

    public static bool IsCanPurchase(CurrencyPair price)
    {
        return ShopService.Current.IsCanPurchase(new Price{Currency = price.Currency, DefaultPriceAmount = price.Amount});
    }

    public static bool IsCanPurchase(List<CurrencyPair> prices)
    {
        var currentPrices = new List<Price>();

        foreach (var price in prices)
        {
            currentPrices.Add(new Price{Currency = price.Currency, DefaultPriceAmount = price.Amount});
        }

        return ShopService.Current.IsCanPurchase(currentPrices);
    }

    public static int GetCountByTag(CurrencyTag tag)
    {
        var defs = Currency.GetCurrencyDefs(tag);

        var count = defs.Sum(def => ProfileService.Current.GetStorageItem(def.Name).Amount);
        return count;
    }
}