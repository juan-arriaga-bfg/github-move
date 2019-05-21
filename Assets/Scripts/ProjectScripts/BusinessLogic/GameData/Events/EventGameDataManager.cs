using System;
using System.Collections.Generic;
using UnityEngine;

public class EventGameDataManager : IECSComponent, IDataManager, IDataLoader<List<EventGameStepDef>>
{
    public static int ComponentGuid = ECSManager.GetNextGuid();
    public int Guid => ComponentGuid;
    
    public Dictionary<EventGameType, List<EventGameStepDef>> Defs;
    private List<EventGame> eventGames = new List<EventGame>();
    
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
        Defs = new Dictionary<EventGameType, List<EventGameStepDef>>();
        LoadData(new HybridConfigDataMapper<List<EventGameStepDef>>("configs/eventOrderSoftLaunch.data", NSConfigsSettings.Instance.IsUseEncryption));
    }
	
    public void LoadData(IDataMapper<List<EventGameStepDef>> dataMapper)
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

                        if (real != null) continue;
                        
                        def.RealPrices.Add(price.Copy());
                    }
                }
                
                Defs.Add(EventGameType.OrderSoftLaunch, data);
            }
            else
            {
                Debug.LogWarningFormat("[{0}]: config not loaded", GetType());
            }
        });
        
        var serverData = ServerSideConfigService.Current.GetData<List<GameEventServerConfig>>();
        
        if (serverData == null) return;
        
        foreach (var config in serverData)
        {
            if (Enum.TryParse<EventGameType>(config.Type, false, out var gameType) == false || gameType != EventGameType.OrderSoftLaunch) continue;
            
            var game = new EventGame {EventType = EventGameType.OrderSoftLaunch};
            
            game.InitData(config.Start, config.End, config.IntroDuration);
            eventGames.Add(game);
        }
    }

    public List<EventGame> GetNewEventGame()
    {
        var result = eventGames;

        eventGames = new List<EventGame>();
        
        return result;
    }
}