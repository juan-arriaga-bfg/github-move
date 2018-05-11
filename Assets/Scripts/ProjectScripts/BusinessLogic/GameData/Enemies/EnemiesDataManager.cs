using System.Collections.Generic;
using UnityEngine;

public class EnemiesDataManager : IDataLoader<List<EnemyDef>>
{
    public List<EnemyDef> Enemies;
    public List<int> DeadEnemies = new List<int>();

    public Enemy CurrentEnemy;
    
    public void LoadData(IDataMapper<List<EnemyDef>> dataMapper)
    {
        dataMapper.LoadData((data, error)=> 
        {
            if (string.IsNullOrEmpty(error))
            {
                Enemies = data;
                
                var save = ProfileService.Current.GetComponent<CharacterSaveComponent>(CharacterSaveComponent.ComponentGuid);
                
                if (save != null && save.DeadEnemies != null)
                {
                    DeadEnemies = save.DeadEnemies;
                    CurrentEnemy = GetEnemy();

                    if (save.Enemies != null && save.Enemies.Count != 0)
                    {
                        CurrentEnemy.Init(save.Enemies[0]);
                    }
                    return;
                }
                
                CurrentEnemy = GetEnemy();
            }
            else
            {
                Debug.LogWarning("[EnemiesDataManager]: enemies config not loaded");
            }
        });
    }

    public Enemy GetEnemy()
    {
        if (CurrentEnemy != null)
        {
            return CurrentEnemy;
        }
        
        var last = DeadEnemies.Count == 0 ? 0 : DeadEnemies[DeadEnemies.Count - 1];
        
        var index = Enemies.FindIndex(def => def.Uid == last);
        index = index == -1 || index + 1 == Enemies.Count ? 0 : index + 1;
        
        if(index == 0) DeadEnemies = new List<int>();
        
        return new Enemy(Enemies[index]);
    }
    
    public void Kill(int uid)
    {
        DeadEnemies.Add(uid);
        CurrentEnemy = null;
        CurrentEnemy = GetEnemy();
    }
}