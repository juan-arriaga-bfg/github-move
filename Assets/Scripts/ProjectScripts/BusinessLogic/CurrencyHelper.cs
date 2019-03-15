using Debug = IW.Logger;
using System;
using System.Collections.Generic;
using UnityEngine;

public static partial class CurrencyHelper
{
    public static ShopItemTransaction Purchase(string product, int amount, Action<bool> onSuccess = null, Vector3? flyPosition = null)
    {
        return Purchase(product, amount, Currency.Cash.Name, 0, onSuccess, flyPosition);
    }
    
    public static ShopItemTransaction Purchase(string product, int amount, CurrencyPair price, Action<bool> onSuccess = null, Vector3? flyPosition = null)
    {
        return Purchase(product, amount, price.Currency, price.Amount, onSuccess, flyPosition);
    }
    
    public static ShopItemTransaction Purchase(CurrencyPair product, Action<bool> onSuccess = null, Vector3? flyPosition = null)
    {
        return Purchase(product, new CurrencyPair{Currency = Currency.Cash.Name, Amount = 0}, onSuccess, flyPosition);
    }
    
    public static void Purchase(List<CurrencyPair> products, Action<bool> onSuccess = null, Vector3? flyPosition = null)
    {
        for (var i = 0; i < products.Count; i++)
        {
            var product = products[i];
            
            var shopItem = new ShopItem
            {
                Uid = $"purchase.test.{product.Currency}.10", 
                ItemUid = product.Currency, 
                Amount = product.Amount,
                CurrentPrices = new List<Price>{new Price{Currency = Currency.Cash.Name, DefaultPriceAmount = 0}}
            };

            var isLast = i == products.Count - 1;
            
            PurchaseItem(shopItem, product, false, isLast ? onSuccess : null, flyPosition, 0.5f * i);
        }
    }
    
    public static void Purchase(List<CurrencyPair> products, CurrencyPair price, Action<bool> onSuccess = null, Vector3? flyPosition = null)
    {
        for (var i = 0; i < products.Count; i++)
        {
            var product = products[i];
            
            var isLast = i == products.Count - 1;
            
            var shopItem = new ShopItem
            {
                Uid = $"purchase.test.{product.Currency}.10", 
                ItemUid = product.Currency, 
                Amount = product.Amount,
                CurrentPrices = new List<Price>{new Price{Currency = (isLast ? price.Currency : Currency.Cash.Name), DefaultPriceAmount = (isLast ? price.Amount : 0)}}
            };
            
            PurchaseItem(shopItem, product, false, isLast ? onSuccess : null, flyPosition, 0.5f * i);
        }
    }
    
    public static ShopItemTransaction Purchase(CurrencyPair product, CurrencyPair price, Action<bool> onSuccess = null, Vector3? flyPosition = null)
    {
        return Purchase(product.Currency, product.Amount, price.Currency, price.Amount, onSuccess, flyPosition);
    }
    
    public static ShopItemTransaction Purchase(string product, int amountProduct, string price, int amountPrice, Action<bool> onSuccess = null, Vector3? flyPosition = null)
    {
        var shopItem = new ShopItem
        {
            Uid = $"purchase.test.{product}.10", 
            ItemUid = product, 
            Amount = amountProduct,
            CurrentPrices = new List<Price>{new Price{Currency = price, DefaultPriceAmount = amountPrice}}
        };

        return PurchaseItem(shopItem, new CurrencyPair{Currency = product, Amount = amountProduct}, true, onSuccess, flyPosition);
    }
    
    public static ShopItemTransaction Purchase(CurrencyPair product, List<CurrencyPair> prices, Action<bool> onSuccess = null, Vector3? flyPosition = null)
    {
        return Purchase(product.Currency, product.Amount, prices, onSuccess, flyPosition);
    }
    
    public static ShopItemTransaction Purchase(string product, int amount, List<CurrencyPair> prices, Action<bool> onSuccess = null, Vector3? flyPosition = null)
    {
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

        return PurchaseItem(shopItem, new CurrencyPair {Currency = product, Amount = amount}, false, onSuccess, flyPosition);
    }
    
    private static ShopItemTransaction PurchaseItem(ShopItem shopItem, CurrencyPair product, bool isShowHint, Action<bool> onSuccess = null, Vector3? flyPosition = null, float delay = 0)
    {
        Debug.Log($"[CurrencyHelper] => PurchaseItem: shopItem: {shopItem.ItemUid}: {shopItem.Amount}, product: {product}");
        
        var isSuccess = false;
        
        var transaction = ShopService.Current.PurchaseItem
        (
            shopItem,
            (item, s) =>
            {
                // on purchase success
                isSuccess = true;
                
                if (flyPosition != null)
                {
                    CurrencyFly(flyPosition.Value, product, onSuccess, delay);
                    return;
                }
                
                onSuccess?.Invoke(true);
            },
            item =>
            {
                // on purchase failed (not enough cash)
                onSuccess?.Invoke(false);
                
                if (!isShowHint) return;
                
                var prices = shopItem.CurrentPrices[0];
                IsCanPurchase(prices.Currency, prices.DefaultPriceAmount, true);
            },
            false
        );
        
        return transaction;
    }
}