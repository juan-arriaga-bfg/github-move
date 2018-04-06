﻿using System.Collections.Generic;

public class Hero
{
    private HeroDef def;
    
    public readonly CurrencyDef LevelCurrencyDef;
    public readonly CurrencyDef CardCurrencyDef;
    
    private int inAdventure = -1;

    public int Level
    {
        get
        {
            return LevelCurrencyDef == null ? 0 : (int)ProfileService.Current.GetStorageItem(LevelCurrencyDef.Name).Amount;
        }
    }
    
    public Hero(HeroDef def)
    {
        this.def = def;
        
        LevelCurrencyDef = Currency.GetCurrencyDef(string.Format("Level{0}", def.Uid));
        CardCurrencyDef = Currency.GetCurrencyDef(string.Format("{0}Card", def.Uid));

        foreach (var level in def.Levels)
        {
            level.Abilities.Sort((a, b) => -a.Value.CompareTo(b.Value));
        }
    }
    
    public HeroDef Def
    {
        get { return def; }
    }
    
    public int InAdventure
    {
        get { return inAdventure; }
        set { inAdventure = value; }
    }
    
    public int CurrentProgress
    {
        get { return ProfileService.Current.GetStorageItem(CardCurrencyDef.Name).Amount; }
    }

    public int TotalProgress
    {
        get
        {
            var price = def.Levels[Level].Prices.Find(pair => pair.Currency == CardCurrencyDef.Name);
            
            return price == null ? 0 : price.Amount;
        }
    }
    
    public int Price
    {
        get
        {
            var price = def.Levels[Level].Prices.Find(pair => pair.Currency == Currency.Coins.Name);
            
            return price == null ? 0 : price.Amount;
        }
    }

    public AbilityType CurrentAbility
    {
        get {return def.Levels[Level].Abilities[0].Ability; }
    }

    public int CurrentAbilityValue
    {
        get { return def.Levels[Level].Abilities[0].Value; }
    }

    public int GetAbilityValue(AbilityType ability)
    {
        var heroAbility = def.Levels[Level].Abilities.Find(pair => pair.Ability == ability);
        
        return heroAbility == null ? 0 : heroAbility.Value;
    }
    
    public int NextAbilityValue
    {
        get
        {
            var level = Level;
            return level == def.Levels.Count - 1 ? 0 : def.Levels[level + 1].Abilities[0].Value;
        }
    }

    public bool IsLevelMax()
    {
        var tavernLevel = ProfileService.Current.GetStorageItem(Currency.LevelTavern.Name).Amount;

        return Level + 1 > tavernLevel;
    }
    
    public void LevelUp()
    {
        var prices = def.Levels[Level].Prices;
        var currentPrices = new List<Price>();

        foreach (var price in prices)
        {
            currentPrices.Add(new Price{Currency = price.Currency, DefaultPriceAmount = price.Amount});
        }
        
        var shopItem = new ShopItem
        {
            Uid = string.Format("purchase.test.{0}.10", LevelCurrencyDef.Name), 
            ItemUid = LevelCurrencyDef.Name, 
            Amount = 1,
            CurrentPrices = currentPrices
        };
        
        ShopService.Current.PurchaseItem
        (
            shopItem,
            (item, s) =>
            {
                // on purchase success
                AddPower();
            },
            item =>
            {
                // on purchase failed (not enough cash)
                var model2= UIService.Get.GetCachedModel<UIMessageWindowModel>(UIWindowType.MessageWindow);
        
                model2.Title = "Need coins?";
                model2.Message = null;
                model2.Image = "tutorial_TextBlock_1";
                model2.AcceptLabel = "Ok";
        
                model2.OnAccept = () => {};
                model2.OnCancel = null;
        
                UIService.Get.ShowWindow(UIWindowType.MessageWindow);
            }
        );
    }

    private void AddPower()
    {
        var shopItem = new ShopItem
        {
            Uid = string.Format("purchase.test.{0}.10", Currency.Power.Name), 
            ItemUid = Currency.Power.Name, 
            Amount = GetAbilityValue(AbilityType.Power),
            CurrentPrices = new List<Price>{new Price{Currency = Currency.Cash.Name, DefaultPriceAmount = 0}}
        };
        
        ShopService.Current.PurchaseItem
        (
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