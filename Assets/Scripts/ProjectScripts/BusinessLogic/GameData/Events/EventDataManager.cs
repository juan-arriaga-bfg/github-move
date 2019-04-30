using System;
using System.Collections.Generic;
using UnityEngine;

public enum EventName
{
    OrderSoftLaunch,
}

public class EventDataManager : IECSComponent, IDataManager, IDataLoader<List<EventStepDef>>
{
    public static int ComponentGuid = ECSManager.GetNextGuid();
    public int Guid => ComponentGuid;
    
    public Dictionary<EventName, List<EventStepDef>> Defs;

    public Action<EventName> OnStart;
    public Action<EventName> OnStop;
    
    public void OnRegisterEntity(ECSEntity entity)
    {
        Reload();
    }
	
    public void OnUnRegisterEntity(ECSEntity entity)
    {
    }

    public void Reload()
    {
        Defs = new Dictionary<EventName, List<EventStepDef>>();
        LoadData(new ResourceConfigDataMapper<List<EventStepDef>>("configs/eventOrderSoftLaunch.data", NSConfigsSettings.Instance.IsUseEncryption));
    }
	
    public void LoadData(IDataMapper<List<EventStepDef>> dataMapper)
    {
        dataMapper.LoadData((data, error) =>
        {
            if (string.IsNullOrEmpty(error))
            {
                Defs.Add(EventName.OrderSoftLaunch, data);
            }
            else
            {
                Debug.LogWarningFormat("[{0}]: config not loaded", GetType());
            }
        });
    }

    public void Start(string id)
    {
        var name = (EventName)Enum.Parse(typeof(EventName), id);
        OnStart?.Invoke(name);
    }
    
    public void Stop(string id)
    {
        var name = (EventName)Enum.Parse(typeof(EventName), id);
        OnStop?.Invoke(name);
    }

    public bool IsStarted(EventName name)
    {
        return false;
    }

    public bool IsPremium(EventName name)
    {
        return false;
    }
}