using System.Collections.Generic;
using UnityEngine;

public class HeroesDataManager : IDataLoader<List<HeroDef>>
{
    public List<HeroDef> Heroes;
    
    public void LoadData(IDataMapper<List<HeroDef>> dataMapper)
    {
        dataMapper.LoadData((data, error)=> 
        {
            if (string.IsNullOrEmpty(error))
            {
                Heroes = data;
            }
            else
            {
                Debug.LogWarning("[HeroesDataManager]: heroes config not loaded");
            }
        });
    }
    
    public int HeroLevel
    {
        get
        {
            var hero = GetHero("Robin");
            var level = Mathf.Clamp(ProfileService.Current.GetStorageItem(Currency.Level.Name).Amount, 0, hero.Damages.Count - 1);
            
            return level;
        }
    }
    
    public HeroDef GetHero(string uid)
    {
        return Heroes.Find(def => def.Uid == uid);
    }
}