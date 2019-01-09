using System.Collections.Generic;
using UnityEngine;

public class MarketDataManager : IECSComponent, IDataManager, IDataLoader<List<MarketDef>>
{
    public static int ComponentGuid = ECSManager.GetNextGuid();
    public int Guid => ComponentGuid;

    private List<MarketDefParrent> defs;

    public void OnRegisterEntity(ECSEntity entity)
    {
        Reload();
    }

    public void OnUnRegisterEntity(ECSEntity entity)
    {
    }
	
    public void Reload()
    {
        defs = new List<MarketDefParrent>();
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
                    var parrent = defs.Find(p => p.Uid == def.Uid);

                    if (parrent == null)
                    {
                        parrent = new MarketDefParrent {Uid = def.Uid};
                        defs.Add(parrent);
                    }
					
                    parrent.AddDef(def);
                }
            }
            else
            {
                Debug.LogWarningFormat("[{0}]: config not loaded", GetType());
            }
        });
    }

    public List<MarketDef> GetSlotsData()
    {
        var result = new List<MarketDef>();

        foreach (var parrent in defs)
        {
            result.Add(parrent.GetDef());
        }
        
        return result;
    }
}