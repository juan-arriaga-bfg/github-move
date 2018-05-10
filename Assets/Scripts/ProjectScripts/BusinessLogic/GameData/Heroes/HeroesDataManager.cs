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
                var save = ProfileService.Current.GetComponent<CharacterSaveComponent>(CharacterSaveComponent.ComponentGuid);
                
                foreach (var def in data)
                {
                    var hero = new Hero(def);
                    
                    Heroes.Add(hero);

                    if (save == null || save.Heroes == null) continue;
                    
                    var item = save.Heroes.Find(h => h.Id == def.SaveIndex());
                    
                    if(item == null) continue;
                    
                    hero.Sleep(item.StartTime);
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