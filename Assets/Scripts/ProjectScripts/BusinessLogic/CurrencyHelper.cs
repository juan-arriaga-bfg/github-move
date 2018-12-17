using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public static class CurrencyHellper
{
    public static ShopItemTransaction Purchase(string product, int amount, Action<bool> onSuccess = null, Vector3? flyPosition = null)
    {
        return Purchase(product, amount, Currency.Cash.Name, 0, onSuccess, flyPosition);
    }
    
    public static ShopItemTransaction PurchaseAsync(string product, int amount, Action<bool> onSuccess = null, Vector3? flyPosition = null)
    {
        return PurchaseAsync(product, amount, Currency.Cash.Name, 0, onSuccess, flyPosition);
    }
    
    public static ShopItemTransaction Purchase(string product, int amount, CurrencyPair price, Action<bool> onSuccess = null, Vector3? flyPosition = null)
    {
        return Purchase(product, amount, price.Currency, price.Amount, onSuccess, flyPosition);
    }
    
    public static ShopItemTransaction Purchase(CurrencyPair product, Action<bool> onSuccess = null, Vector3? flyPosition = null)
    {
        return Purchase(product, new CurrencyPair{Currency = Currency.Cash.Name, Amount = 0}, onSuccess, flyPosition);
    }
    
    public static ShopItemTransaction PurchaseAsync(CurrencyPair product, Action<bool> onSuccess = null, Vector3? flyPosition = null)
    {
        return PurchaseAsync(product, new CurrencyPair{Currency = Currency.Cash.Name, Amount = 0}, onSuccess, flyPosition);
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
    
    public static ShopItemTransaction PurchaseAsync(CurrencyPair product, CurrencyPair price, Action<bool> onSuccess = null, Vector3? flyPosition = null)
    {
        return PurchaseAsync(product.Currency, product.Amount, price.Currency, price.Amount, onSuccess, flyPosition);
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
        
        return PurchaseItem(shopItem, new CurrencyPair{Currency = product, Amount = amount}, false,onSuccess, flyPosition);
    }

    
    private static ShopItemTransaction PurchaseItem(ShopItem shopItem, CurrencyPair product, bool isShoowHint, Action<bool> onSuccess = null, Vector3? flyPosition = null, float delay = 0)
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
                
                if (!isShoowHint) return;
                
                var prices = shopItem.CurrentPrices[0];
                IsCanPurchase(prices.Currency, prices.DefaultPriceAmount, true);
            },
            false
        );
        
        return transaction;
    }
    
    private static ShopItemTransaction PurchaseItemAsync(ShopItem shopItem, CurrencyPair product, bool isShoowHint, Action<bool> onSuccess = null, Vector3? flyPosition = null, float delay = 0)
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
                
                if (!isShoowHint) return;
                
                var prices = shopItem.CurrentPrices[0];
                IsCanPurchase(prices.Currency, prices.DefaultPriceAmount, true);
            },
            true
        );
        
        return transaction;
    }
    
    private static void CurrencyFly(Vector3 screenPosition, CurrencyPair resource, Action<bool> onSuccess = null, float delay = 0)
    {
        var fly = ResourcesViewManager.Instance.GetFirstViewById(resource.Currency);
        
        if (fly == null) return;
        
        ResourcePanelUtils.ToggleFadePanel(resource.Currency, true);
        
        var carriers = ResourcesViewManager.DeliverResource<ResourceCarrier>
        (
            resource.Currency,
            resource.Amount,
            fly.GetAnchorRect(),
            screenPosition,
            R.ResourceCarrier,
            delay
        );
        
        carriers[carriers.Count - 1].Callback = () =>
        {
            ResourcePanelUtils.ToggleFadePanel(resource.Currency, false);
            onSuccess?.Invoke(true);
        };
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

    public static bool IsCanPurchase(CurrencyPair price, out int diff, bool isMessageShow = false, Action onMessageConfirm = null)
    {
        return IsCanPurchase(price.Currency, price.Amount, out diff, isMessageShow, onMessageConfirm);
    }
    
    public static bool IsCanPurchase(CurrencyPair price, bool isMessageShow = false, Action onMessageConfirm = null)
    {
        var diff = 0;
        return IsCanPurchase(price.Currency, price.Amount, out diff, isMessageShow, onMessageConfirm);
    }
    
    public static bool IsCanPurchase(string price, int amount, bool isMessageShow = false, Action onMessageConfirm = null)
    {
        var diff = 0;
        return IsCanPurchase(price, amount, out diff, isMessageShow, onMessageConfirm);
    }
    
    public static bool IsCanPurchase(string price, int amount, out int diff, bool isMessageShow = false, Action onMessageConfirm = null)
    {
        diff = 0;
        
        if (string.IsNullOrEmpty(price)) return false;
        if (price == Currency.Cash.Name) return true;
        
        var storageItem = ProfileService.Current.Purchases.GetStorageItem(price);

        if (storageItem.Amount >= amount) return true;
        
        diff = amount - storageItem.Amount;

        if (isMessageShow)
        {
            Debug.Log($"[CurrencyHelper] => Show 'Not enough' message for: {price}: {amount}");
            
            if (price == Currency.Crystals.Name)
            {
                UIMessageWindowController.CreateNeedCurrencyMessage(price, diff.ToString());
            }
            else
            {
                OpenShopWindow(price, diff.ToString());
            }
        }
        
        return false;
    }
    
    public static bool IsCanPurchase(List<CurrencyPair> prices, bool isMessageShow = false, Action onMessageConfirm = null)
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

    public static void OpenShopWindow(string currency, string diff = "")
    {
        BoardService.Current.GetBoardById(0)?.BoardEvents.RaiseEvent(GameEventsCodes.ClosePieceUI, null);
        
        if (currency == Currency.Coins.Name)
        {
            var model = UIService.Get.GetCachedModel<UISoftShopWindowModel>(UIWindowType.SoftShopWindow);

            model.ShopType = Currency.Coins;
            
            UIService.Get.ShowWindow(UIWindowType.SoftShopWindow);
            return;
        }
        
        if (currency == Currency.Energy.Name)
        {
            var model = UIService.Get.GetCachedModel<UISoftShopWindowModel>(UIWindowType.SoftShopWindow);

            model.ShopType = Currency.Energy;
            
            UIService.Get.ShowWindow(UIWindowType.SoftShopWindow);
            return;
        }
        
        if (currency == Currency.Crystals.Name)
        {
            var product = new CurrencyPair{Currency = currency, Amount = 5};
            var message = string.Format(LocalizationService.Get("common.message.cheatCurrency", "common.message.cheatCurrency {0}?"), product.ToStringIcon());
        
            UIMessageWindowController.CreateDefaultMessage(message, () =>
            {
                Purchase(product, null, new Vector2(Screen.width/2, Screen.height/2));
            });
            return;
        }
        
        UIMessageWindowController.CreateNeedCurrencyMessage(currency, diff);
    }
    
    public static CurrencyPair ResourcePieceToCurrence(Dictionary<int, int> dict, string currency)
    {
        var amount = 0;
        
        foreach (var pair in dict)
        {
            var def = GameDataService.Current.PiecesManager.GetPieceDef(pair.Key);
            
            if(def?.SpawnResources == null || def.SpawnResources.Currency != currency) continue;
            
            amount += pair.Value * def.SpawnResources.Amount;
        }
        
        return new CurrencyPair{Currency = currency, Amount = amount};
    }
    
    public static Dictionary<int, int> CurrencyToResourcePieces(int amount, string currency)
    {
        var dict = new Dictionary<int, int>();
        var ids = PieceType.GetIdsByFilter(PieceTypeFilter.Resource);
        var defs = new List<PieceDef>();

        foreach (var id in ids)
        {
            var def = GameDataService.Current.PiecesManager.GetPieceDef(id);
            
            if(def?.SpawnResources == null || def.SpawnResources.Currency != currency) continue;
            
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

    public static Dictionary<int, int> FiltrationRewards(List<CurrencyPair> src, out List<CurrencyPair> currency)
    {
        var dict = new Dictionary<int, int>();
        currency = new List<CurrencyPair>();

        foreach (var reward in src)
        {
            var id = PieceType.Parse(reward.Currency);

            if (id == PieceType.None.Id)
            {
                currency.Add(reward);
                continue;
            }
            
            if (dict.ContainsKey(id) == false)
            {
                dict.Add(id, reward.Amount);
                continue;
            }
            
            dict[id] += reward.Amount;
        }

        return dict;
    }

    public static string RewardsToString(string separator, Dictionary<int, int> pieces, List<CurrencyPair> currencys, bool noAmount = false)
    {
        var types = new List<string>();
        var rewards = new List<string>();
        
        foreach (var pair in currencys)
        {
            rewards.Add(pair.ToStringIcon());
        }
            
        foreach (var reward in pieces)
        {
            var def = GameDataService.Current.PiecesManager.GetPieceDef(reward.Key);

            if (def?.SpawnResources == null)
            {
                rewards.Add(new CurrencyPair{Currency = PieceType.Parse(reward.Key), Amount = reward.Value}.ToStringIcon(noAmount));
                continue;
            }
                
            var currency = def.SpawnResources.Currency;
                
            if(types.Contains(currency)) continue;
                
            var pair = ResourcePieceToCurrence(pieces, currency);
            
            if (pair.Amount == 0) pair.Amount = reward.Value;
                
            types.Add(currency);
            rewards.Add(pair.ToStringIcon());
        }
        
        return string.Join(separator, rewards);
    }
}