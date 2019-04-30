using System;
using System.Collections.Generic;
using System.Text;
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

    private GameDataManager context;
    
    public void OnRegisterEntity(ECSEntity entity)
    {
        context = entity as GameDataManager;
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
                for (var i = 0; i < data.Count; i++)
                {
                    var def = data[i];
                    var previous = i == 0 ? new List<CurrencyPair>() : data[i - 1].RealPrices;
                    
                    def.RealPrices = new List<CurrencyPair>();
                    
                    foreach (var prev in previous)
                    {
                        var real = prev.Copy();
                        var price = def.Prices.Find(pair => pair.Currency == real.Currency);

                        real.Amount += price?.Amount ?? 0;
                        def.RealPrices.Add(real.Copy());
                    }

                    foreach (var price in def.Prices)
                    {
                        var real = def.RealPrices.Find(pair => pair.Currency == price.Currency);
                        
                        if(real != null) continue;
                        
                        def.RealPrices.Add(price.Copy());
                    }
                }
                
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

    public int GetStepProgress(EventName name, int step, int space)
    {
        var progressSecond = 0;
        var defs = Defs[name];
        var current = context.UserProfile.GetStorageItem(Currency.Token.Name).Amount;

        foreach (var def in defs)
        {
            if (current < def.RealPrices[0].Amount)
            {
                var value = def.RealPrices[0].Amount - current;

                progressSecond += (int) (step * (1 - value / (float) def.Prices[0].Amount));
                break;
            }
            
            progressSecond += space + step;
        }

        return progressSecond;
    }
}