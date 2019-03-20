using System;
using System.Collections.Generic;
using UnityEngine;

public static partial class CurrencyHelper
{
    public static ShopItemTransaction PurchaseAsync(string product, int amount, Action<bool> onSuccess = null, Vector3? flyPosition = null)
    {
        return PurchaseAsync(product, amount, Currency.Cash.Name, 0, onSuccess, flyPosition);
    }
    
    public static ShopItemTransaction PurchaseAsync(CurrencyPair product, Action<bool> onSuccess = null, Vector3? flyPosition = null)
    {
        return PurchaseAsync(product.Currency, product.Amount, Currency.Cash.Name, 0, onSuccess, flyPosition);
    }
    
    public static List<ShopItemTransaction> PurchaseAsync(List<CurrencyPair> products, Action<bool> onSuccess = null, Vector3? flyPosition = null)
    {
        var transactions = new List<ShopItemTransaction>();
        
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
            
            var transaction = PurchaseItemAsync(shopItem, product, false, isLast ? onSuccess : null, flyPosition, 0.5f * i);
            
            transactions.Add(transaction);
        }

        return transactions;
    }
    
    public static ShopItemTransaction PurchaseAsync(CurrencyPair product, CurrencyPair price, Action<bool> onSuccess = null, Vector3? flyPosition = null)
    {
        return PurchaseAsync(product.Currency, product.Amount, price.Currency, price.Amount, onSuccess, flyPosition);
    }
    
    public static ShopItemTransaction PurchaseAsync(string product, int amountProduct, string price, int amountPrice, Action<bool> onSuccess = null, Vector3? flyPosition = null)
    {
        var shopItem = new ShopItem
        {
            Uid = $"purchase.test.{product}.10", 
            ItemUid = product, 
            Amount = amountProduct,
            CurrentPrices = new List<Price>{new Price{Currency = price, DefaultPriceAmount = amountPrice}}
        };

        return PurchaseItemAsync(shopItem, new CurrencyPair{Currency = product, Amount = amountProduct}, true, onSuccess, flyPosition);
    }
    
    private static ShopItemTransaction PurchaseItemAsync(ShopItem shopItem, CurrencyPair product, bool isShowHint, Action<bool> onSuccess = null, Vector3? flyPosition = null, float delay = 0)
    {
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
            true
        );
        
        return transaction;
    }
}