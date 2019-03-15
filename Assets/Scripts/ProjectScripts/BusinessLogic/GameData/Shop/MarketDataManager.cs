using Debug = IW.Logger;
using System;
using System.Collections.Generic;
using UnityEngine;

public class MarketDataManager : IECSComponent, IDataManager, IDataLoader<List<MarketDef>>
{
    public static int ComponentGuid = ECSManager.GetNextGuid();
    public int Guid => ComponentGuid;

    public List<MarketItem> Defs;

    public Action UpdateState;

    private ECSEntity context;

    public void OnRegisterEntity(ECSEntity entity)
    {
        context = entity;
        
        Reload();
    }

    public void OnUnRegisterEntity(ECSEntity entity)
    {
        context = null;
    }
	
    public void Reload()
    {
        Defs = new List<MarketItem>();
        LoadData(new ResourceConfigDataMapper<List<MarketDef>>("configs/market.data", NSConfigsSettings.Instance.IsUseEncryption));
    }

    public void LoadData(IDataMapper<List<MarketDef>> dataMapper)
    {
        dataMapper.LoadData((data, error) =>
        {
            if (string.IsNullOrEmpty(error))
            {
                foreach (var def in data)
                {
                    var item = Defs.Find(p => p.Uid == def.Uid);

                    if (item == null)
                    {
                        item = new MarketItem {Uid = def.Uid};
                        Defs.Add(item);
                    }
					
                    item.AddDef(def);
                }
                
                var save = ((GameDataManager)context).UserProfile.GetComponent<MarketSaveComponent>(MarketSaveComponent.ComponentGuid);
                
                if(save?.Slots == null || save.Slots.Count == 0) return;
                
                for (var i = 0; i < Defs.Count; i++)
                {
                    var item = Defs[i];
                    var slot = save.Slots.Find(saveItem => saveItem.Index == i);

                    if (slot == null) continue;

                    item.Init(slot.ItemIndex, slot.Piece, slot.Amount, slot.State);
                }
            }
            else
            {
                Debug.LogWarningFormat("[{0}]: config not loaded", GetType());
            }
        });
    }

    public void UpdateSlots(bool isTimer = false)
    {
        if (BoardService.Current?.FirstBoard == null) return;
        
        foreach (var def in Defs)
        {
            def.Update(isTimer);
        }
    }
}