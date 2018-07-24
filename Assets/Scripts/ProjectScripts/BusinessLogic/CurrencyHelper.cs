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

    public static bool IsCanPurchase(List<CurrencyPair> prices, out List<CurrencyPair> diffs)
    {
        diffs = new List<CurrencyPair>();
        
        foreach (var price in prices)
        {
            int diff = 0;
            
            if(IsCanPurchase(price, out diff)) continue;
            
            diffs.Add(new CurrencyPair{Currency = price.Currency, Amount = diff});
        }
        
        return diffs.Count == 0;
    }

    public static bool IsCanPurchase(CurrencyPair price, out int diff)
    {
        return IsCanPurchase(price.Currency, price.Amount, out diff);
    }

    public static bool IsCanPurchase(string price, int amount, out int diff)
    {
        diff = 0;
        
        if (price == null) return false;

        if (price == Currency.Cash.Name) return true;
        
        var storageItem = ProfileService.Current.Purchases.GetStorageItem(price);

        if (storageItem.Amount >= amount)
        {
            return true;
        }

        diff = amount - storageItem.Amount;
        return false;
    }
    
    public static bool IsCanPurchase(string price, int amount, bool isMessageShow = false)
    {
        var isCan = ShopService.Current.IsCanPurchase(new Price{Currency = price, DefaultPriceAmount = amount});

        if (isCan == false && isMessageShow)
        {
            if(price == Currency.Coins.Name) UIMessageWindowController.CreateNeedCoinsMessage();
            else UIMessageWindowController.CreateNeedCurrencyMessage(price);
        }
        
        return isCan;
    }

    public static bool IsCanPurchase(CurrencyPair price, bool isMessageShow = false)
    {
        var isCan = ShopService.Current.IsCanPurchase(new Price{Currency = price.Currency, DefaultPriceAmount = price.Amount});
        
        if (isCan == false && isMessageShow)
        {
            if(price.Currency == Currency.Coins.Name) UIMessageWindowController.CreateNeedCoinsMessage();
            else UIMessageWindowController.CreateNeedCurrencyMessage(price.Currency);
        }
        
        return isCan;
    }

    public static bool IsCanPurchase(List<CurrencyPair> prices, bool isMessageShow = false)
    {
        var isCan = true;
        
        foreach (var price in prices)
        {
            if (IsCanPurchase(price, isMessageShow)) continue;
            
            isCan = false;
            break;
        }

        return isCan;
    }

    public static int GetCountByTag(CurrencyTag tag)
    {
        var defs = Currency.GetCurrencyDefs(tag);

        var count = defs.Sum(def => ProfileService.Current.GetStorageItem(def.Name).Amount);
        return count;
    }

    public static CurrencyPair CoinPieceToCurrence(Dictionary<int, int> dict)
    {
        var amount = 0;
        
        foreach (var pair in dict)
        {
            var def = GameDataService.Current.PiecesManager.GetPieceDef(pair.Key);
            
            if(def == null) continue;
            
            amount += pair.Value * def.SpawnResources.Amount;
        }
        
        return new CurrencyPair{Currency = Currency.Coins.Name, Amount = amount};
    }
    
    public static Dictionary<int, int> CurrencyToCoinPieces(int amount)
    {
        var dict = new Dictionary<int, int>();
        
        for (var id = PieceType.Coin5.Id; id >= PieceType.Coin1.Id && amount > 0; id--)
        {
            var def = GameDataService.Current.PiecesManager.GetPieceDef(id);
            
            if(def == null) continue;
            
            var count = amount / def.SpawnResources.Amount;
            
            if(count == 0) continue;
            
            amount -= count * def.SpawnResources.Amount;
            dict.Add(id, count);
        }
        
        return dict;
    }

    public static Dictionary<int, int> MinimizeCoinPieces(Dictionary<int, int> dict)
    {
        return CurrencyToCoinPieces(CoinPieceToCurrence(dict).Amount);
    }
}