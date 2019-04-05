using System.Collections.Generic;
using UnityEngine;

public class DailyRewardDataManager : IECSComponent, IDataManager, IDataLoader<List<DailyRewardDef>>
{
    public static int ComponentGuid = ECSManager.GetNextGuid();
    public int Guid => ComponentGuid;

    public List<DailyRewardDef> Defs;

    public void OnRegisterEntity(ECSEntity entity)
    {
        Reload();
    }

    public void OnUnRegisterEntity(ECSEntity entity)
    {
    }
	
    public void Reload()
    {
        Defs = new List<DailyRewardDef>();
        LoadData(new ResourceConfigDataMapper<List<DailyRewardDef>>("configs/daily.data", NSConfigsSettings.Instance.IsUseEncryption));
    }

    public void LoadData(IDataMapper<List<DailyRewardDef>> dataMapper)
    {
        dataMapper.LoadData((data, error) =>
        {
            if (string.IsNullOrEmpty(error))
            {
                Defs = data;
            }
            else
            {
                Debug.LogWarningFormat("[{0}]: config not loaded", GetType());
            }
        });
    }
}