using System.Collections.Generic;
using UnityEngine;

public class HeroesDataManager : IDataLoader<List<HeroDef>>
{
    public List<Hero> Heroes;
    
    public void LoadData(IDataMapper<List<HeroDef>> dataMapper)
    {
        dataMapper.LoadData((data, error)=> 
        {
            Heroes = new List<Hero>();
            
            if (string.IsNullOrEmpty(error))
            {
                foreach (var def in data)
                {
                    Heroes.Add(new Hero(def));
                }
            }
            else
            {
                Debug.LogWarning("[HeroesDataManager]: heroes config not loaded");
            }
        });
    }
    
    public Hero GetHero(string uid)
    {
        return Heroes.Find(hero => hero.Def.Uid == uid);
    }
    
    public Hero GetHero(AbilityType ability)
    {
        return Heroes.Find(hero => hero.CurrentAbility == ability);
    }
    
    public List<Hero> GetHeroes(AbilityType ability)
    {
        return Heroes.FindAll(hero => hero.CurrentAbility == ability);
    }
    
    public Hero GetHeroByCurrency(string currensy)
    {
        return Heroes.Find(h => h.CardCurrencyDef.Name == currensy);
    }

    public int CurrentPower()
    {
        return ProfileService.Current.GetStorageItem(Currency.Power.Name).Amount;
    }
}