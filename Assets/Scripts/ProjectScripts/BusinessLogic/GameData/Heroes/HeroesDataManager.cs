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

    public int CurrentPower()
    {
        return ProfileService.Current.GetStorageItem(Currency.Power.Name).Amount;
    }
}