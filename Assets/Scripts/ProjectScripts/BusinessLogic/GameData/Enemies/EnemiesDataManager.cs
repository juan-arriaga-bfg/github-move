using System.Collections.Generic;
using UnityEngine;

public class EnemiesDataManager : IECSComponent, IDataManager, IDataLoader<List<EnemyDef>>
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
    
    private Dictionary<int, EnemyDef> enemyDefs;

    public void Reload()
    {
        enemyDefs = null;
        LoadData(new ResourceConfigDataMapper<List<EnemyDef>>("configs/enemies.data", NSConfigsSettings.Instance.IsUseEncryption));
    }
    
    public void LoadData(IDataMapper<List<EnemyDef>> dataMapper)
    {
        dataMapper.LoadData((data, error)=> 
        {
            enemyDefs = new Dictionary<int, EnemyDef>();

            if (string.IsNullOrEmpty(error))
            {
                foreach (var next in data)
                {
                    enemyDefs.Add(next.Piece, next);
                }
            }
            else
            {
                Debug.LogWarningFormat("[{0}]: config not loaded", GetType());
            }
        });
    }

    public EnemyDef GetEnemyDefById(int id)
    {
        return enemyDefs[id];
    }
}