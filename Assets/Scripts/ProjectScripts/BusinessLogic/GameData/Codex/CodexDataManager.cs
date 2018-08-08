using System.Collections.Generic;
using UnityEngine;

public class CodexDataManager : IECSComponent, IDataManager, IDataLoader<List<CodexChainState>>
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();

    public int Guid => ComponentGuid;

    public Dictionary<int, CodexChainState> Items = new Dictionary<int, CodexChainState>();

    private MatchDefinitionComponent matchDef; 
    
    public void OnRegisterEntity(ECSEntity entity)
    {
        Reload();
    }
    
    public void OnUnRegisterEntity(ECSEntity entity)
    {
    }
	
    public void Reload()
    {
        matchDef = null;
        
        Items = new Dictionary<int, CodexChainState>();
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
    
    public void LoadData(IDataMapper<List<CodexChainState>> dataMapper)
    {
        
    }

    public void OnPieceBuilded(int id)
    {
        if (IsPieceUnlocked(id))
        {
            return;
        }
        
        CodexChainState state;
        if (Items.TryGetValue(id, out state))
        {
            state.Unlocked = Mathf.Max(state.Unlocked, id);
        }
        
        Debug.LogWarning($"[CodexDataManager] => OnPieceBuilded({id}) can't find corresponded chain.");
    }

    public bool IsPieceUnlocked(int id)
    {
        if (matchDef == null)
        {
            var board = BoardService.Current.GetBoardById(0);
            matchDef = board.BoardLogic.GetComponent<MatchDefinitionComponent>(MatchDefinitionComponent.ComponentGuid);
        }
        
        int firstInChain = matchDef.GetFirst(id);
        
        CodexChainState state;
        if (Items.TryGetValue(id, out state))
        {
            return state.Unlocked <= id;
        }

        Debug.LogWarning($"[CodexDataManager] => IsPieceUnlocked({id}) can't find corresponded chain.");
        return true;
    }
}