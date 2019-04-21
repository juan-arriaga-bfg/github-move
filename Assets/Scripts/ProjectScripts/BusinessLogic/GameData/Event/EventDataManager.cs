using System.Collections.Generic;
using UnityEngine;

public class EventDataManager : IECSComponent, IDataManager, IDataLoader<List<EventStepyDef>>
{
    public static int ComponentGuid = ECSManager.GetNextGuid();
    public int Guid => ComponentGuid;
    
    public List<EventStepyDef> Defs;
    
    public void OnRegisterEntity(ECSEntity entity)
    {
        Reload();
    }
	
    public void OnUnRegisterEntity(ECSEntity entity)
    {
    }

    public void Reload()
    {
        Defs = new List<EventStepyDef>();
        LoadData(new ResourceConfigDataMapper<List<EventStepyDef>>("configs/event.data", NSConfigsSettings.Instance.IsUseEncryption));
    }
	
    public void LoadData(IDataMapper<List<EventStepyDef>> dataMapper)
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