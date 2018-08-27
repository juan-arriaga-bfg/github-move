using System.Collections.Generic;
using UnityEngine;

public class PiecesMatchConditionsManager : IECSComponent, IDataManager, IDataLoader<List<PiecesMatchConditionsDef>>
{
    public static int ComponentGuid = ECSManager.GetNextGuid();
    public int Guid => ComponentGuid;

    private PiecesDataManager context;
    
    public void OnRegisterEntity(ECSEntity entity)
    {
        context = entity as PiecesDataManager;
        Reload();
    }

    public void OnUnRegisterEntity(ECSEntity entity)
    {
    }
    
    public void Reload()
    {
        LoadData(new ResourceConfigDataMapper<List<PiecesMatchConditionsDef>>("configs/piecesMatchConditions.data", NSConfigsSettings.Instance.IsUseEncryption));
    }

    public void LoadData(IDataMapper<List<PiecesMatchConditionsDef>> dataMapper)
    {
        dataMapper.LoadData((data, error)=> 
        {
            if (string.IsNullOrEmpty(error))
            {
                foreach (var def in data)
                {
                    var baseDef = context.GetPieceDef(def.Id);
                    
                    if(baseDef == null) continue;

                    baseDef.MatchConditionsDef = def;
                }
            }
            else
            {
                Debug.LogWarningFormat("[{0}]: config not loaded", GetType());
            }
        });
    }
}