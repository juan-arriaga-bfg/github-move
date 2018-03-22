using System.Collections.Generic;
using UnityEngine;

public class HeroesDataManager : IDataLoader<List<HeroDef>>
{
    public List<Hero> Heroes;
    
    public BoardPosition HousePosition { get; set; }
    
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

    public Hero GetHeroByCurrency(string currensy)
    {
        return Heroes.Find(h => h.CardCurrencyDef.Name == currensy);
    }
}