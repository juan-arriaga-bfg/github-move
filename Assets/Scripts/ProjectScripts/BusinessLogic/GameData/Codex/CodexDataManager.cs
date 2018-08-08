using System.Collections.Generic;
using UnityEngine;

public class CodexDataManager : IECSComponent, IDataManager, IDataLoader<Dictionary<int, CodexChainState>>
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
        Items = null;

        LoadData(new ResourceConfigDataMapper<Dictionary<int, CodexChainState>>("configs/codex.data", NSConfigsSettings.Instance.IsUseEncryption));
    }

    public void LoadData(IDataMapper<Dictionary<int, CodexChainState>> dataMapper)
    {
        dataMapper.LoadData((data, error) =>
                            {
                                if (string.IsNullOrEmpty(error))
                                {
                                    var save = ProfileService.Current.GetComponent<CodexSaveComponent>(CodexSaveComponent.ComponentGuid);

                                    Items = save?.Data != null && save.Data.Count > 0 ? save.Data : data;
                                }
                                else
                                {
                                    Debug.LogWarningFormat("[{0}]: config not loaded", GetType());
                                }
                            });
    }

    public void LoadData(IDataMapper<List<CodexChainState>> dataMapper)
    {

    }

    /// <summary>
    /// Returns true if new piece unlocked
    /// </summary>
    public bool OnPieceBuilded(int id)
    {
        if (IsPieceUnlocked(id))
        {
            return false;
        }

        int firstInChain = matchDef.GetFirst(id);

        CodexChainState state;
        if (Items.TryGetValue(firstInChain, out state))
        {
            if (state.Unlocked.Add(id))
            {
                state.PendingReward.Add(id);
            }
        }
        else
        {
            Items.Add(firstInChain,
                      new CodexChainState
                      {
                          Unlocked = new HashSet<int> {id}, 
                          PendingReward = new HashSet<int> {id}
                      }
            );
        }

        return true;
    }

    public bool IsPieceUnlocked(int id)
    {
        if (Items == null)
        {
            Debug.LogError($"[CodexDataManager] => IsPieceUnlocked({id}) access to saved data before it actually loaded!");
            return true;
        }

        if (matchDef == null)
        {
            var board = BoardService.Current.GetBoardById(0);
            matchDef = board.BoardLogic.GetComponent<MatchDefinitionComponent>(MatchDefinitionComponent.ComponentGuid);
        }

        int firstInChain = matchDef.GetFirst(id);

        CodexChainState state;
        if (Items.TryGetValue(firstInChain, out state))
        {
            return state.Unlocked.Contains(id);
        }

        Debug.LogWarning($"[CodexDataManager] => IsPieceUnlocked({id}) can't find corresponded chain.");
        return true;
    }

    public bool GetChainState(int firstId, out CodexChainState state)
    {
        if (Items == null)
        {
            Debug.LogError($"[CodexDataManager] => GetChainState({firstId}) access to saved data before it actually loaded!");
            state = null;
            return false;
        }

        return Items.TryGetValue(firstId, out state);
    }

    // todo: cache it
    public bool IsAnyPendingReward()
    {
        foreach (var item in Items.Values)
        {
            if (item.PendingReward.Count > 0)
            {
                return true;
            }
        }

        return false;
    }
}