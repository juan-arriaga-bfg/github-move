using System.Collections.Generic;

public class Hero
{
    private HeroDef def;
    
    public Hero(HeroDef def)
    {
        this.def = def;
    }
    
    public void Collect()
    {
        var prices = new List<CurrencyPair>();
        
        prices.AddRange(def.Price);
        prices.AddRange(def.Collection);
        
        CurrencyHellper.Purchase(def.Uid, 1, prices, success =>
        {
            if (success)
            {
                CurrencyHellper.Purchase(Currency.Power.Name, GetAbilityValue(AbilityType.Power));
                return;
            }
            
            var model2= UIService.Get.GetCachedModel<UIMessageWindowModel>(UIWindowType.MessageWindow);
        
            model2.Title = "Need coins?";
            model2.Message = null;
            model2.Image = "tutorial_TextBlock_1";
            model2.AcceptLabel = "Ok";
        
            model2.OnAccept = () => {};
            model2.OnCancel = null;
        
            UIService.Get.ShowWindow(UIWindowType.MessageWindow);
        });
    }
    
    public int GetAbilityValue(AbilityType ability)
    {
        var heroAbility = def.Abilities.Find(pair => pair.Ability == ability);
        
        return heroAbility == null ? 0 : heroAbility.Value;
    }
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
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
            return 0;
           /* var price = def.Levels[Level].Prices.Find(pair => pair.Currency == CardCurrencyDef.Name);
            
            return price == null ? 0 : price.Amount;*/
        }
    }
    
    public int Price
    {
        get
        {
            return 0;
            /*var price = def.Levels[Level].Prices.Find(pair => pair.Currency == Currency.Coins.Name);
            
            return price == null ? 0 : price.Amount;*/
        }
    }

    public AbilityType CurrentAbility
    {
        get
        {
            return 0;
//            return def.Levels[Level].Abilities[0].Ability;
        }
    }

    public int CurrentAbilityValue
    {
        get
        {
            return 0;
//            return def.Levels[Level].Abilities[0].Value;
        }
    }
    
    public int NextAbilityValue
    {
        get
        {
            return 0;
//            var level = Level;
//            return level == def.Levels.Count - 1 ? 0 : def.Levels[level + 1].Abilities[0].Value;
        }
    }

    public bool IsLevelMax()
    {
        var tavernLevel = ProfileService.Current.GetStorageItem(Currency.LevelTavern.Name).Amount;

        return Level + 1 > tavernLevel;
    }
    
    
}