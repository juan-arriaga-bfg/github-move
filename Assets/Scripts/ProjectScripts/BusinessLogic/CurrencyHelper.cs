using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public static class CurrencyHellper
{
    public static bool Purchase(string product, int amount, Action<bool> onSuccess = null, Vector3? flyPosition = null)
    {
        return Purchase(product, amount, Currency.Cash.Name, 0, onSuccess, flyPosition);
    }
    
    public static bool Purchase(string product, int amount, CurrencyPair price, Action<bool> onSuccess = null, Vector3? flyPosition = null)
    {
        return Purchase(product, amount, price.Currency, price.Amount, onSuccess, flyPosition);
    }
    
    public static bool Purchase(CurrencyPair product, Action<bool> onSuccess = null, Vector3? flyPosition = null)
    {
        return Purchase(product, new CurrencyPair{Currency = Currency.Cash.Name, Amount = 0}, onSuccess, flyPosition);
    }
    
    public static void Purchase(List<CurrencyPair> products, Vector3? flyPosition = null)
    {
        for (var i = 0; i < products.Count; i++)
        {
            var product = products[i];
            Purchase(product, new CurrencyPair{Currency = Currency.Cash.Name, Amount = 0});
            
            if (flyPosition == null) continue;

            DOTween.Sequence()
                .AppendInterval(0.5f * i)
                .AppendCallback(() => CurrencyFly(flyPosition.Value, new CurrencyPair {Currency = product.Currency, Amount = product.Amount}));
        }
    }
    
    public static bool Purchase(CurrencyPair product, CurrencyPair price, Action<bool> onSuccess = null, Vector3? flyPosition = null)
    {
        return Purchase(product.Currency, product.Amount, price.Currency, price.Amount, onSuccess, flyPosition);
    }
    
    public static bool Purchase(string product, int amountProduct, string price, int amountPrice, Action<bool> onSuccess = null, Vector3? flyPosition = null)
    {
        var isSuccess = false;
        
        var shopItem = new ShopItem
        {
            Uid = $"purchase.test.{product}.10", 
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
                if (flyPosition != null) CurrencyFly(flyPosition.Value, new CurrencyPair{Amount = amountProduct, Currency = product});
                onSuccess?.Invoke(true);

            },
            item =>
            {
                // on purchase failed (not enough cash)
                onSuccess?.Invoke(false);

                ShowHint(price);
            }
        );

        return isSuccess;
    }

    private static void CurrencyFly(Vector3 screenPosition, CurrencyPair resource)
    {
        
        var fly = ResourcesViewManager.Instance.GetFirstViewById(resource.Currency);
        if (fly == null) return;
        
        var carriers = ResourcesViewManager.DeliverResource<ResourceCarrier>
        (
            resource.Currency,
            resource.Amount,
            fly.GetAnchorRect(),
            screenPosition,
            R.ResourceCarrier
        );
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
            Uid = $"purchase.test.{product}.10", 
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
                onSuccess?.Invoke(true);
            },
            item =>
            {
                // on purchase failed (not enough cash)
                onSuccess?.Invoke(false);
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
            ShowHint(price);
        }
        
        return isCan;
    }

    public static bool IsCanPurchase(CurrencyPair price, bool isMessageShow = false)
    {
        var isCan = ShopService.Current.IsCanPurchase(new Price{Currency = price.Currency, DefaultPriceAmount = price.Amount});
        
        if (isCan == false && isMessageShow)
        {
            ShowHint(price.Currency);
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

    public static CurrencyPair ResourcePieceToCurrence(Dictionary<int, int> dict, string currensy)
    {
        var amount = 0;
        
        foreach (var pair in dict)
        {
            var def = GameDataService.Current.PiecesManager.GetPieceDef(pair.Key);
            
            if(def == null
               || def.SpawnResources == null
               || def.SpawnResources.Currency != currensy) continue;
            
            amount += pair.Value * def.SpawnResources.Amount;
        }
        
        return new CurrencyPair{Currency = currensy, Amount = amount};
    }
    
    public static Dictionary<int, int> CurrencyToResourcePieces(int amount, string currensy)
    {
        var dict = new Dictionary<int, int>();
        var ids = PieceType.GetIdsByFilter(PieceTypeFilter.Resource);
        var defs = new List<PieceDef>();

        foreach (var id in ids)
        {
            var def = GameDataService.Current.PiecesManager.GetPieceDef(id);
            
            if(def == null
               || def.SpawnResources == null
               || def.SpawnResources.Currency != currensy) continue;
            
            defs.Add(def);
        }
        
        defs.Sort((a, b) => a.SpawnResources.Amount.CompareTo(b.SpawnResources.Amount));

        for (var i = defs.Count - 1; i >= 0 && amount > 0; i--)
        {
            var def = defs[i];
            var count = amount / def.SpawnResources.Amount;
            
            if(count == 0) continue;
            
            amount -= count * def.SpawnResources.Amount;
            dict.Add(PieceType.Parse(def.SpawnResources.Currency), count);
        }
        
        return dict;
    }

    private static void ShowHint(string currency)
    {
        if (currency == Currency.Worker.Name)
        {
            return;
        }
        
        if (currency == Currency.Coins.Name)
        {
            UIMessageWindowController.CreateNeedCoinsMessage();
            return;
        }
        
        if (currency == Currency.Energy.Name)
        {
            UIService.Get.ShowWindow(UIWindowType.EnergyShopWindow);
            return;
        }
        
        UIMessageWindowController.CreateNeedCurrencyMessage(currency);
    }
}