using System.Collections.Generic;

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
        get { return def.Price; }
    }
    
    public List<CurrencyPair> Collection
    {
        get { return def.Collection; }
    }
    
    public Hero(HeroDef def)
    {
        this.def = def;
    }

    public bool IsCollect
    {
        get { return ProfileService.Current.GetStorageItem(def.Uid).Amount > 0; }
    }
    
    public void Collect()
    {
        if (IsCollect) return;
        
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
            
            UIMessageWindowController.CreateNeedCoinsMessage();
        });
    }
    
    public int GetAbilityValue(AbilityType ability)
    {
        var heroAbility = def.Abilities.Find(pair => pair.Ability == ability);
        
        return heroAbility == null ? 0 : heroAbility.Value;
    }
}