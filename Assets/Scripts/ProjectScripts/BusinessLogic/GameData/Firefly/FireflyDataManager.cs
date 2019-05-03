using System.Collections.Generic;
using UnityEngine;

public class FireflyDataManager : IECSComponent, IDataManager, IDataLoader<List<FireflyDef>>
{
    public static int ComponentGuid = ECSManager.GetNextGuid();
    public int Guid => ComponentGuid;
    
    public Dictionary<FireflyLogicType, FireflyDef> Defs;
    
    public void OnRegisterEntity(ECSEntity entity)
    {
        Reload();
    }
	
    public void OnUnRegisterEntity(ECSEntity entity)
    {
    }

    public void Reload()
    {
        Defs = new Dictionary<FireflyLogicType, FireflyDef>();
        LoadData(new ResourceConfigDataMapper<List<FireflyDef>>("configs/firefly.data", NSConfigsSettings.Instance.IsUseEncryption));
    }
	
    public void LoadData(IDataMapper<List<FireflyDef>> dataMapper)
    {
        dataMapper.LoadData((data, error) =>
        {
            if (string.IsNullOrEmpty(error))
            {
                foreach (var def in data)
                {
                    Defs.Add(def.Uid, def);
                }
            }
            else
            {
                Debug.LogWarningFormat("[{0}]: config not loaded", GetType());
            }
        });
    }
}