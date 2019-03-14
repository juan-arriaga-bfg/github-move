using System;
using System.Collections.Generic;
using UnityEngine;

public static partial class CurrencyHelper
{
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

    public static bool IsCanPurchase(CurrencyPair price, out int diff, bool isMessageShow = false)
    {
        return IsCanPurchase(price.Currency, price.Amount, out diff, isMessageShow);
    }
    
    public static bool IsCanPurchase(CurrencyPair price, bool isMessageShow = false)
    {
        var diff = 0;
        return IsCanPurchase(price.Currency, price.Amount, out diff, isMessageShow);
    }
    
    public static bool IsCanPurchase(string price, int amount, bool isMessageShow = false)
    {
        var diff = 0;
        return IsCanPurchase(price, amount, out diff, isMessageShow);
    }
    
    public static bool IsCanPurchase(string price, int amount, out int diff, bool isMessageShow = false)
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
            
            OpenShopWindow(price, diff.ToString());
        }
        
        return false;
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
    
    public static void CurrencyFly(Vector3 screenPosition, CurrencyPair resource, Action<bool> onSuccess = null, float delay = 0)
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

        carriers[0].Callback += () =>
        {
            if(resource.Currency == Currency.Coins.Name)
                NSAudioService.Current.Play(SoundId.GetSoftCurr, false, 1);
            if (resource.Currency == Currency.Energy.Name)
                NSAudioService.Current.Play(SoundId.GetEnergy, false, 1);
            if(resource.Currency == Currency.Mana.Name)
                NSAudioService.Current.Play(SoundId.GetMagic, false, 1);
            if(resource.Currency == Currency.Crystals.Name)
                NSAudioService.Current.Play(SoundId.GetHardCurr, false, 1);
        };
        
        foreach (var resourceCarrier in carriers)
        {
            resourceCarrier.Callback += () =>
            {
                if(resource.Currency == Currency.Experience.Name) NSAudioService.Current.Play(SoundId.GetXp);
            };
        }
    }
    
    public static void OpenShopWindow(string currency, string diff = "")
    {
        BoardService.Current.FirstBoard?.BoardEvents.RaiseEvent(GameEventsCodes.ClosePieceUI, null);
        
        if (currency == Currency.Energy.Name)
        {
            UIService.Get.ShowWindow(UIWindowType.EnergyShopWindow);
            return;
        }
        
        if (currency == Currency.Coins.Name)
        {
            if (BoardService.Current.FirstBoard.TutorialLogic.CheckMarket() == false)
            {
                UIMessageWindowController.CreateMessage(
                    LocalizationService.Get("common.title.forbidden", "common.title.forbidden"),
                    LocalizationService.Get("common.message.forbidden.market", "common.message.forbidden.market"));
            
                return;
            }
            
            UIService.Get.ShowWindow(UIWindowType.MarketWindow);
            return;
        }
        
        if (currency == Currency.Crystals.Name)
        {
            UIService.Get.ShowWindow(UIWindowType.HardShopWindow);
            return;
        }
        
        UIMessageWindowController.CreateNeedCurrencyMessage(currency, diff);
    }
    
    public static CurrencyPair ResourcePieceToCurrency(Dictionary<int, int> dict, string currency)
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
            dict.Add(def.Id, count);
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

    public static string RewardsToString(string separator, Dictionary<int, int> pieces, List<CurrencyPair> currencies, bool noAmount = false)
    {
        var types = new List<string>();
        var rewards = new List<string>();
        
        foreach (var pair in currencies)
        {
            rewards.Add(pair.ToStringIcon());
        }

        if (pieces != null)
        {
            foreach (var reward in pieces)
            {
                var def = GameDataService.Current.PiecesManager.GetPieceDef(reward.Key);

                if (def?.SpawnResources == null)
                {
                    rewards.Add(new CurrencyPair {Currency = PieceType.Parse(reward.Key), Amount = reward.Value}.ToStringIcon(noAmount));
                    continue;
                }

                var currency = def.SpawnResources.Currency;

                if (types.Contains(currency)) continue;

                var pair = ResourcePieceToCurrency(pieces, currency);

                if (pair.Amount == 0) pair.Amount = reward.Value;

                types.Add(currency);
                rewards.Add(pair.ToStringIcon());
            }
        }

        return string.Join(separator, rewards);
    }
}
