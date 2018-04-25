using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Hero
{
    private HeroDef def;

    public HeroDef Def
    {
        get { return def; }
    }

    private int inAdventure = -1;

    public int InAdventure
    {
        get { return inAdventure; }
        set { inAdventure = value; }
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
        get { return Random.Range(0, 2) == 0; }
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
        
    }
    
    public int GetAbilityValue(AbilityType ability)
    {
        var heroAbility = def.Abilities.Find(pair => pair.Ability == ability);
        
        return heroAbility == null ? 0 : heroAbility.Value;
    }
}