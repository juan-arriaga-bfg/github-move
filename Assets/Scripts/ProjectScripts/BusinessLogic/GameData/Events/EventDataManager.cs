using System;
using System.Collections.Generic;
using System.Linq;
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
    
    public int Step => context.UserProfile.GetStorageItem(Currency.EventStep.Name).Amount;
    
    public Action<EventName> OnStart;
    public Action<EventName> OnStop;

    private GameDataManager context;
    
    public void OnRegisterEntity(ECSEntity entity)
    {
        context = entity as GameDataManager;
        Reload();

        ServerSideConfigService.Current.OnDataReceived += OnServerDataReceived;
    }
	
    public void OnUnRegisterEntity(ECSEntity entity)
    {
        ServerSideConfigService.Current.OnDataReceived -= OnServerDataReceived;
    }

    private void OnServerDataReceived(int guid, object data)
    {
        if (guid != GameEventServerSideConfigLoader.ComponentGuid)
        {
            return;
        }

        var serverData = (List<GameEventServerConfig>) data;
        
        // todo SERVER EVENT: 
        // как-нибудь обрабатываем полученные данные, по аналогии с примером в LoadData
    }

    public void Reload()
    {
        Defs = new Dictionary<EventName, List<EventStepDef>>();
        LoadData(new ResourceConfigDataMapper<List<EventStepDef>>("configs/eventOrderSoftLaunch.data", NSConfigsSettings.Instance.IsUseEncryption));
    }
	
    public void LoadData(IDataMapper<List<EventStepDef>> dataMapper)
    {
        // todo SERVER EVENT
        List<GameEventServerConfig> serverData = ServerSideConfigService.Current.GetData<List<GameEventServerConfig>>();
        if (serverData == null)
        {
            // запрос на сервер пока не прошел, ответ будет позже, в колбэке OnServerDataReceived. На данный момент считаем, что никакие данные не поменялись.
        }
        else
        {
            GameEventServerConfig config = serverData.FirstOrDefault(e => e.Type == EventName.OrderSoftLaunch.ToString());
            if (config != null)
            {
                // Тут у нас есть все данные по ивенту. Можем стартовать новый или стопать текущий.
                DateTime startDate = config.Start;
                DateTime endDate = config.End;
                int introDuration = config.IntroDuration;
            }
            else
            {
                // На сервере больше нет такого ивента либо он закончился - стопаем текущий, если он еще в процессе
            }
        }
        // end todo
        
        dataMapper.LoadData((data, error) =>
        {
            if (string.IsNullOrEmpty(error))
            {
                var save = context.UserProfile?.EventSave?.Steps ?? new List<EventSaveItem>();
                
                for (var i = 0; i < data.Count; i++)
                {
                    var def = data[i];
                    var saveStep = save.Find(item => item.Step == def.Step);
                    var previous = i == 0 ? new List<CurrencyPair>() : data[i - 1].RealPrices;

                    if (saveStep != null)
                    {
                        def.IsNormalClaimed = saveStep.IsNormalClaimed;
                        def.IsPremiumClaimed = saveStep.IsPremiumClaimed;
                    }
                    
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
    
    public int Price(EventName name)
    {
        var step = IsCompleted(name) ? Step - 1 : Step;
        return Defs[name][step].Prices[0].Amount;
    }
    
    public bool IsStarted(EventName name)
    {
        return true;
    }

    public bool IsPremium(EventName name)
    {
        return false;
    }
    
    public bool IsLastStep(EventName name)
    {
        return Step == Defs[name].Count - 1;
    }
    
    public bool IsCompleted(EventName name)
    {
        return Step == Defs[name].Count;
    }
}