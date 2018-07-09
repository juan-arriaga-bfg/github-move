using System.Collections.Generic;
using UnityEngine;

public class HeroesDataManager : IECSComponent, IDataManager, IDataLoader<List<HeroDef>>
{
    public static int ComponentGuid = ECSManager.GetNextGuid();

    public int Guid { get { return ComponentGuid; } }
	
    public void OnRegisterEntity(ECSEntity entity)
    {
        Reload();
    }

    public void OnUnRegisterEntity(ECSEntity entity)
    {
    }
    
    public List<Hero> Heroes;
    
    public void Reload()
    {
        Heroes = null;
        LoadData(new ResourceConfigDataMapper<List<HeroDef>>("configs/heroes.data", NSConfigsSettings.Instance.IsUseEncryption));
    }
    
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
                Debug.LogWarningFormat("[{0}]: config not loaded", GetType());
            }
        });
    }

    public int CurrentPower()
    {
        return ProfileService.Current.GetStorageItem(Currency.Power.Name).Amount;
    }
}