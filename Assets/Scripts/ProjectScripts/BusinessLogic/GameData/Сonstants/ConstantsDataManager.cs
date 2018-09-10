using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class ConstantsDataManager : ECSEntity, IDataManager, IDataLoader<List<ConstantsDef>>
{
    public static int ComponentGuid = ECSManager.GetNextGuid();
    public override int Guid => ComponentGuid;
    
    public int CreateManaDelay;
    public int ReproductionDelay;
    public int ReproductionStepDelay;
    public int ReproductionChance;
    
    public override void OnRegisterEntity(ECSEntity entity)
    {
        Reload();
    }
    
    public void Reload()
    {
        LoadData(new ResourceConfigDataMapper<List<ConstantsDef>>("configs/constants.data", NSConfigsSettings.Instance.IsUseEncryption));
    }

    public void LoadData(IDataMapper<List<ConstantsDef>> dataMapper)
    {
        dataMapper.LoadData((data, error)=>
        {
            if (string.IsNullOrEmpty(error))
            {
//                Debug.LogError("!!!!!!!");
                
                var t = typeof(ConstantsDataManager);
                var fieldInfos = t.GetFields(BindingFlags.Public);
        
                for (var i = 0; i < fieldInfos.Length; i++)
                {
                    var fieldInfo = fieldInfos[i];
                    
                    Debug.LogError(fieldInfo.Name);
                }
                
                /*CreateManaDelay = data.CreateManaDelay;
                ReproductionDelay = data.ReproductionDelay;
                ReproductionStepDelay = data.ReproductionStepDelay;
                ReproductionChance = data.ReproductionChance;*/
            }
            else
            {
                Debug.LogWarningFormat("[{0}]: config not loaded", GetType());
            }
        });
    }
}