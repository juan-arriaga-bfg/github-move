using System.Collections.Generic;
using UnityEngine;

public class EnemiesDataManager : IDataLoader<List<EnemyDef>>
{
    public List<EnemyDef> Enemies;
    
    public void LoadData(IDataMapper<List<EnemyDef>> dataMapper)
    {
        dataMapper.LoadData((data, error)=> 
        {
            if (string.IsNullOrEmpty(error))
            {
                Enemies = data;
            }
            else
            {
                Debug.LogWarning("[EnemiesDataManager]: enemies config not loaded");
            }
        });
    }

    public Enemy GetEnemy(int index)
    {
        return new Enemy(Enemies[index]);
    }
}