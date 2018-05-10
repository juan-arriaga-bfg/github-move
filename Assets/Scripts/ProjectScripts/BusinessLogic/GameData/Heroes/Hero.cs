using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Hero
{
    private HeroDef def;
    private DateTime? sleepTime;
    private DateTime? completeTime;

    public HeroDef Def
    {
        get { return def; }
    }
    
    public List<CurrencyPair> Price
    {
        get { return def.Prices; }
    }

    public List<CurrencyPair> Collection
    {
        get { return def.Collection; }
    }

    public Hero(HeroDef def)
    {
        this.def = def;
    }

    public bool IsSleep
    {
        get
        {
            if (IsUnlock == false || IsCollect == false) return true;
            
            return sleepTime != null && (int) (DateTime.UtcNow - sleepTime.Value).TotalSeconds < def.Cooldown;
        }
    }

    public bool IsCollect
    {
        get { return ProfileService.Current.GetStorageItem(def.Uid).Amount > 0; }
    }

    public bool IsUnlock
    {
        get
        {
            return GameDataService.Current.LevelsManager.Level >= def.Level;
        }
    }

    public bool IsCanPurchase
    {
        get
        {
            var prices = new List<CurrencyPair>();
        
            prices.AddRange(def.Prices);
            prices.AddRange(def.Collection);
            
            return CurrencyHellper.IsCanPurchase(prices);
        }
    }
    
    public bool IsCollectChargers
    {
        get { return CurrencyHellper.IsCanPurchase(def.Prices); }
    }

    public bool IsCollectCollections
    {
        get { return CurrencyHellper.IsCanPurchase(def.Collection); }
    }

    public int ChargersPrice
    {
        get
        {
            var price = def.Prices.Sum(pair =>
            {
                if (CurrencyHellper.IsCanPurchase(pair)) return 0;
                return pair.Amount - ProfileService.Current.GetStorageItem(pair.Currency).Amount;
            });
            
            return price;
        }
    }
    
    public bool IsElementOfHero(string element)
    {
        var item = def.Collection.Find(pair => pair.Currency == element);
        
        return item != null;
    }

    public void PurchaseChargers()
    {
        if (IsCollect) return;
        
        foreach (var pair in def.Prices)
        {
            if (CurrencyHellper.IsCanPurchase(pair)) continue;

            var amount = pair.Amount - ProfileService.Current.GetStorageItem(pair.Currency).Amount;
            
            CurrencyHellper.Purchase(pair.Currency, amount, Currency.Crystals.Name, amount);
        }

        Purchase();
    }
    
    public void Purchase()
    {
        if (IsCollect) return;
        
        var prices = new List<CurrencyPair>();
        
        prices.AddRange(def.Prices);
        prices.AddRange(def.Collection);
        
        CurrencyHellper.Purchase(def.Uid, 1, prices, success =>
        {
            if (!success) return;
            
            CurrencyHellper.Purchase(Currency.Power.Name, GetAbilityValue(AbilityType.Power));
        });
    }

    public void WakeUp()
    {
        sleepTime = null;
    }

    public void Sleep()
    {
        sleepTime = DateTime.UtcNow;
        completeTime = DateTime.UtcNow.AddSeconds(def.Cooldown);
    }

    public string GetSlepTime()
    {
        if (completeTime == null) return "00:00";
        
        var time = completeTime.Value - DateTime.UtcNow;
        return string.Format("{0:00}:{1:00}", time.Minutes, time.Seconds);
    }
    
    public int GetAbilityValue(AbilityType ability)
    {
        var heroAbility = def.Abilities.Find(pair => pair.Ability == ability);
        
        return heroAbility == null ? 0 : heroAbility.Value;
    }
}