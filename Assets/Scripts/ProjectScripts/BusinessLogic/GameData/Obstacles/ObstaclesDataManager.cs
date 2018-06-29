using System.Collections.Generic;
using UnityEngine;

public class ObstaclesDataManager : IECSComponent, IDataManager, IDataLoader<List<ObstacleDef>>
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
    
    private List<ObstacleDef> obstacles;
    public List<ObstacleDef> Obstacles
    {
        get { return obstacles; }
    }
    
    public void Reload()
    {
        obstacles = null;
        LoadData(new ResourceConfigDataMapper<List<ObstacleDef>>("configs/obstacles.data", NSConfigsSettings.Instance.IsUseEncryption));
    }
    
    public void LoadData(IDataMapper<List<ObstacleDef>> dataMapper)
    {
        dataMapper.LoadData((data, error)=> 
        {
            if (string.IsNullOrEmpty(error))
            {
                obstacles = data;
            }
            else
            {
                Debug.LogWarningFormat("[{0}]: config not loaded", GetType());
            }
        });
    }
}