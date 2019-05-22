using Debug = IW.Logger;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class ConstantsDataManager : ECSEntity, IDataManager, IDataLoader<List<ConstantsDef>>
{
    public static int ComponentGuid = ECSManager.GetNextGuid();
    public override int Guid => ComponentGuid;
    
    public int MarketUpdateDelay;
    public int MarketUpdatePrice;
    
    public int MinDelayHintArrow;
    public int MaxDelayHintArrow;
    
    public int MinDelayBounceBubble;
    public int MaxDelayBounceBubble;
    
    public int StartLevelFirefly;
    
    public int StartLevelEventGame;
    
    public int MaxOrders;

    public int FreeTimeLimit;
    
    public float HourBasePrice;
    public float DayBasePrice;
    
    public int EnergyRefillDelay;
    
    public int ExtraWorkerDelay;
    
    public int DelayToClaimFreeEnergy;
    
    public int PremiumIslandPrice;

    public override void OnRegisterEntity(ECSEntity entity)
    {
        Reload();
    }
    
    public void Reload()
    {
        LoadData(new HybridConfigDataMapper<List<ConstantsDef>>("configs/constants.data", NSConfigsSettings.Instance.IsUseEncryption));
    }

    public void LoadData(IDataMapper<List<ConstantsDef>> dataMapper)
    {
        dataMapper.LoadData((data, error)=>
        {
            if (string.IsNullOrEmpty(error))
            {
                var t = typeof(ConstantsDataManager);
                var fieldInfos = t.GetFields(BindingFlags.GetField | BindingFlags.Public | BindingFlags.Instance);
                
                foreach (var info in fieldInfos)
                {
                    var item = data.Find(def => def.Key == info.Name);
                    
                    if(item == null) continue;
                    
                    info.SetValue(this, item.Value);
                }
            }
            else
            {
                Debug.LogWarningFormat("[{0}]: config not loaded", GetType());
            }
        });
    }
}