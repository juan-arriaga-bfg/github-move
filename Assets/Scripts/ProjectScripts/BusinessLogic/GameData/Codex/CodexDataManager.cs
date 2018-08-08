using System.Collections.Generic;
using UnityEngine;

public class CodexDataManager : IECSComponent, IDataManager, IDataLoader<List<CodexDef>>
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();

    public int Guid => ComponentGuid;

    public List<CodexDef> Items = new List<CodexDef>();
	
    public void OnRegisterEntity(ECSEntity entity)
    {
        Reload();
    }
    
    public void OnUnRegisterEntity(ECSEntity entity)
    {
    }
	
    public void Reload()
    {
        Items = new List<CodexDef>();
        var save = ProfileService.Current.GetComponent<CodexSaveComponent>(CodexSaveComponent.ComponentGuid);
        if (save != null)
        {
            Items = save.Data;
        }
        //LoadData(new ResourceConfigDataMapper<List<CodexDef>>("configs/codex.data", NSConfigsSettings.Instance.IsUseEncryption));
    }
    
    // public void LoadData(IDataMapper<List<CodexDef>> dataMapper)
    // {
    //     dataMapper.LoadData((data, error) =>
    //         {
    //             if (string.IsNullOrEmpty(error))
    //             {
    //                 Items = data;
    //
    //                 var save = ProfileService.Current.GetComponent<CodexSaveComponent>(CodexSaveComponent.ComponentGuid);
    //
    //                 if (save != null)
    //                 {
    //                     
    //                 }
    //             }
    //             else
    //             {
    //                 Debug.LogWarningFormat("[{0}]: config not loaded", GetType());
    //             }
    //         });
    // }
    
    public void LoadData(IDataMapper<List<CodexDef>> dataMapper)
    {
        
    }
}