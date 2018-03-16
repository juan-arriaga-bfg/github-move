using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero
{
    private HeroDef def;
    public HeroDef Def
    {
        get { return def; }
    }

    public BoardPosition HousePosition { get; set; }

    public readonly CurrencyDef LevelCurrencyDef;
    public readonly CurrencyDef CardCurrencyDef;

    public int Level
    {
        get
        {
            return LevelCurrencyDef == null ? 0 : (int)ProfileService.Current.GetStorageItem(LevelCurrencyDef.Name).Amount;
        }
    }

    private int inAdventure = -1;

    public int InAdventure
    {
        get { return inAdventure; }
        set { inAdventure = value; }
    }

    public Hero(HeroDef def)
    {
        this.def = def;
        LevelCurrencyDef = Currency.GetCurrencyDef(string.Format("Level{0}", def.Uid));
        CardCurrencyDef = Currency.GetCurrencyDef(string.Format("{0}Card", def.Uid));
    }
    
    public int CurrentProgress
    {
        get { return ProfileService.Current.GetStorageItem(CardCurrencyDef.Name).Amount; }
    }

    public int TotalProgress
    {
        get { return def.Prices[Level].Amount; }
    }

    public int CurrentTimeBonus
    {
        get { return def.TimeBonuses[Level]; }
    }
    
    public int NextTimeBonus
    {
        get
        {
            var level = Level;
            return level == def.TimeBonuses.Count - 1 ? 0 : def.TimeBonuses[level + 1];;
        }
    }
    
    public bool LevelUp()
    {
        var price = def.Prices[Level];
        
        var shopItem = new ShopItem
        {
            Uid = string.Format("purchase.test.{0}.10", LevelCurrencyDef.Name), 
            ItemUid = LevelCurrencyDef.Name, 
            Amount = 1,
            CurrentPrices = new List<Price>
            {
                new Price{Currency = price.Currency, DefaultPriceAmount = price.Amount}
            }
        };

        var isSuccess = false;
        
        ShopService.Current.PurchaseItem
        (
            shopItem,
            (item, s) =>
            {
                // on purchase success
                isSuccess = true;
            },
            item =>
            {
                // on purchase failed (not enough cash)
                isSuccess = false;
            }
        );
        
        return isSuccess;
    }
}