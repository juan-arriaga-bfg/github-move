// #define FORCE_UNLOCK_ALL

using System;
using System.Collections.Generic;
using BfgAnalytics;
using UnityEngine;

public partial class CodexDataManager : IECSComponent, IDataManager, IDataLoader<Dictionary<int, CodexChainState>>
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();

    public int Guid => ComponentGuid;

    public Dictionary<int, CodexChainState> Items = new Dictionary<int, CodexChainState>();

    private MatchDefinitionComponent cachedMatchDef;

    private CodexContent codexContentCache = null;
    
    public Action OnNewItemUnlocked;
    
    public Action<int> OnPieceRewardClaimed;
    
    public CodexState CodexState { get; private set; }= CodexState.Normal;
    
    public readonly List<PieceTypeFilter> hidedPieceFilters = new List<PieceTypeFilter>
    {
        PieceTypeFilter.Fake,
        PieceTypeFilter.Obstacle,
        PieceTypeFilter.Mine
    }; 
        
    public readonly HashSet<int> hidedPieceIds = new HashSet<int>
    {
        PieceType.LockedEmpty.Id,
        PieceType.Boost_WR.Id,
        PieceType.CH_Free.Id,
        PieceType.CH_NPC.Id,
        PieceType.NPC_SleepingBeautyPlaid.Id,
        PieceType.NPC_Gnome.Id,
    }; 
    
    public void OnRegisterEntity(ECSEntity entity)
    {
        Reload();
    }

    public void OnUnRegisterEntity(ECSEntity entity)
    {
    }

    public void Reload()
    {
        cachedMatchDef = null;
               
        Items = null;
        ClearCodexContentCache();

        // Add records to configs/codex.data to unlock any piece in codex at the begining of the game: {"100": {"Unlocked": [100],"PendingReward": []}, ... }
        LoadData(new ResourceConfigDataMapper<Dictionary<int, CodexChainState>>("configs/codex.data", NSConfigsSettings.Instance.IsUseEncryption));
    }

    public void LoadData(IDataMapper<Dictionary<int, CodexChainState>> dataMapper)
    {
        dataMapper.LoadData((data, error) =>
        {
            if (string.IsNullOrEmpty(error))
            {
                var save = ProfileService.Current.GetComponent<CodexSaveComponent>(CodexSaveComponent.ComponentGuid);
                if (save?.Data == null || save.Data.Count == 0)
                {
                    Items = data;

                    // Unlock all pieces on game field...
                    var pieces = GameDataService.Current.FieldManager.Pieces;
                    foreach (var key in pieces.Keys)
                    {
                        UnlockPiece(key);
                    }

                    // ... and do not reward player for them
                    foreach (var item in Items)
                    {
                        item.Value.PendingReward.Clear();
                    }

                    CodexState = CodexState.Normal;
                }
                else
                {
                    Items = save.Data;
                    CodexState = save.State;
                }
            }
            else
            {
                Debug.LogWarningFormat("[{0}]: config not loaded", GetType());
            }
        });
    }

    private MatchDefinitionComponent CachedMatchDef()
    {
        if (cachedMatchDef != null)
        {
            return cachedMatchDef;
        }
        
        cachedMatchDef = BoardService.Current?.FirstBoard?.BoardLogic?.GetComponent<MatchDefinitionComponent>(MatchDefinitionComponent.ComponentGuid) ?? new MatchDefinitionComponent(new MatchDefinitionBuilder().Build());

        return cachedMatchDef;
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

        UnlockPiece(id);

        SendAnalyticsEvent(id);

        ClearCodexContentCache();

        OnNewItemUnlocked?.Invoke();
        
        GameDataService.Current.QuestsManager.StartNewQuestsIfAny();

        return true;
    }

    private static void SendAnalyticsEvent(int id)
    {
        string idStr = PieceType.Parse(id);

        var pieceTypeDef = PieceType.GetDefById(id);

        if (pieceTypeDef.Filter.Has(PieceTypeFilter.Character))
        {
            Analytics.SendCharUnlockedEvent(idStr);
        }
        else
        {
            Analytics.SendPieceUnlockedEvent(idStr);
        }
    }

    private void UnlockPiece(int id)
    {
        int firstInChain = CachedMatchDef().GetFirst(id);

        // Debug.Log($"UnlockPiece: first in chain for {id} is {firstInChain}");
        
        CodexChainState state;
        if (Items.TryGetValue(firstInChain, out state))
        {
            if (state.Unlocked.Add(id))
            {
                state.PendingReward.Add(id);
                CodexState = CodexState.PendingReward;
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
            CodexState = CodexState.PendingReward;
        }
    }

    public bool IsHidedFromCodex(int id)
    {
        if (hidedPieceIds.Contains(id))
        {
            return true;
        }
        
        var def = PieceType.GetDefById(id);

        for (var i = 0; i < hidedPieceFilters.Count; i++)
        {
            var filter = hidedPieceFilters[i];
            if (def.Filter.Has(filter))
            {
                return true;
            }
        }

        return false;
    }

    public bool IsPieceUnlocked(int id)
    {
        if (IsHidedFromCodex(id))
        {
            return true;
        }

        if (Items == null)
        {
            Debug.LogError($"[CodexDataManager] => IsPieceUnlocked({id}) access to saved data before it actually loaded!");
            return true;
        }

        int firstInChain = CachedMatchDef().GetFirst(id);

        CodexChainState state;
        if (Items.TryGetValue(firstInChain, out state))
        {
            return state.Unlocked.Contains(id);
        }

        return false;
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

    public List<CodexItemDef> GetCodexItemsForChainAndFocus(int targetId, int length, bool hideCaptions)
    {
        var board    = BoardService.Current.FirstBoard;
        var matchDef = board.BoardLogic.GetComponent<MatchDefinitionComponent>(MatchDefinitionComponent.ComponentGuid);

        var chain = matchDef.GetChain(targetId);

        // Current piece is not a part of any chain
        if (chain.Count == 0)
        {
            return null;
        }
        
        var list = GetCodexItemsForChain(chain);
        
        int highlightedIndex = -1;

        for (var i = 0; i < list.Count; i++)
        {
            var def = list[i];
            if (def.State == CodexItemState.PendingReward)
            {
                def.State = CodexItemState.Unlocked;
            }

            if (def.PieceTypeDef.Id == targetId)
            {
                if(def.State == CodexItemState.Unlocked)
                    def.State = CodexItemState.Highlighted;
                highlightedIndex = i;
            }

            def.HideCaption = hideCaptions;
        }

        // Put HL to almost end of the displayed part of a chain
        const int ITEMS_TO_SHOW_AFTER_HL = 1;

        int rangeLength;
        int rangeStart;

        if (length >= list.Count)
        {
            rangeLength = list.Count;
            rangeStart = 0;
        }
        else
        {
            rangeLength = Mathf.Min(length, list.Count);
            rangeStart  = Mathf.Max(0, highlightedIndex + 1 - length + ITEMS_TO_SHOW_AFTER_HL);

            if (rangeStart + rangeLength > list.Count)
            {
                rangeStart = Mathf.Max(0, list.Count - rangeLength);
            }
        }

        // Debug.Log($"GetCodexItemsForChainAndFocus: rangeStart: {rangeStart}, rangeLength: {rangeLength}, list.Count: {list.Count}, highlightedIndex: {highlightedIndex}");
        
        List<CodexItemDef> ret = list.GetRange(rangeStart, rangeLength);
        return ret;
    }

    public List<CodexItemDef> GetCodexItemsForChain(List<List<int>> chain)
    {
        var result = new List<CodexItemDef>();

        foreach (var item in chain)
        {
            result.AddRange(GetCodexItemsForChain(item));
        }
        
        return result;
    }
    
    public List<CodexItemDef> GetCodexItemsForChain(List<int> chain)
    {
        // Debug.Log($"========\nGet items: {chain[0]}");
        
        List<CodexItemDef> ret = new List<CodexItemDef>();

        var pieceManager = GameDataService.Current.PiecesManager;

        CodexChainState chainState;
        GameDataService.Current.CodexManager.GetChainState(chain[0], out chainState);

        bool isPreviousPieceUnlocked = false;
        
        for (var i = 0; i < chain.Count; i++)
        {
            int pieceId = chain[i];

            // Debug.Log($"Get items: {pieceId}");
            
            bool isUnlocked = chainState?.Unlocked.Contains(pieceId) ?? false;
            
#if FORCE_UNLOCK_ALL
            isUnlocked = true;
#endif
            bool isPendingReward = chainState?.PendingReward.Contains(pieceId) ?? false;
            
            PieceDef pieceDef = pieceManager.GetPieceDef(pieceId);
            PieceTypeDef pieceTypeDef = PieceType.GetDefById(pieceId);

            // Skip fake
            if (pieceTypeDef.Filter.Has(PieceTypeFilter.Fake))
            {
                continue;
            }
            
            // pieceDef.Reproduction // Create by timer
            // pieceDef.SpawnResources // Create on consume 
            
            CodexItemDef itemDef = new CodexItemDef
            {
                PieceDef = pieceDef,
                PieceTypeDef = pieceTypeDef,
                ShowArrow = i != chain.Count - 1,
                PendingReward = isPendingReward ? pieceDef?.UnlockBonus : null,
            };
            
            if (isUnlocked)
            {
                itemDef.State = isPendingReward ? CodexItemState.PendingReward : CodexItemState.Unlocked;
            } 
            else if (isPreviousPieceUnlocked || i == 0)
            {
                itemDef.State = CodexItemState.PartLock; 
            } 
            else
            {
                itemDef.State = CodexItemState.FullLock; 
            }
            
            if (itemDef.PendingReward != null)
            {
                if (itemDef.PendingReward.Count != 1 || itemDef.PendingReward[0].Currency != Currency.Coins.Name)
                {
                    Debug.LogError($"Codex supports only Coins as reward. Multiply rewards for one item also not supported. Please check config for pieceId: {pieceId}");
                }
            }
            
            // To enable CodexItemState.PartLock for the next item
            isPreviousPieceUnlocked = isUnlocked;
            
            ret.Add(itemDef);
        }                                                                    
        
        // Ensure that no locked items between unlocked
        bool unlockedFound = false;
        for (int i = ret.Count - 1; i >= 0; i--)
        {
            var item = ret[i];
            if (!unlockedFound && (item.State == CodexItemState.Unlocked || item.State == CodexItemState.PendingReward))
            {
                unlockedFound = true;
                continue;
            }

            if (unlockedFound)
            {
                if (item.State == CodexItemState.FullLock)
                {
                    item.State = CodexItemState.PartLock;
                }
            }
        }

        return ret;
    }
    
    public CodexContent GetCodexContent()
    {
        if (codexContentCache != null)
        {
            return codexContentCache;
        }
        
        codexContentCache = BuildContent();
        return codexContentCache;
    }

    public void ClearCodexContentCache()
    {
        codexContentCache = null;
    }
    
    public void ClaimRewardForPiece(int pieceId)
    {
        var chainId = CachedMatchDef().GetFirst(pieceId);
        
        CodexChainState chainState;
        if (Items.TryGetValue(chainId, out chainState))
        {
            chainState.PendingReward.Remove(pieceId);
        }
        else
        {
            Debug.LogError($"[CodexDataManager] => RemovePendingRewardForItem({pieceId}): Can't find CodexChainState for chainId: {chainId}");
        }

        ValidateCodexState();
        
        ClearCodexContentCache();
        
        OnPieceRewardClaimed?.Invoke(pieceId);
    }

    private void ValidateCodexState()
    {
        if (CodexState == CodexState.PendingReward)
        {
            bool isAnyRewardRemaining = false;
            foreach (var item in Items)
            {
                // Skip chains hidden from player
                if (GetCodexContent().GetChainDefByFirstItemId(item.Key) == null)
                {
                    continue;
                }
                
                if (item.Value.PendingReward.Count > 0)
                {
                    isAnyRewardRemaining = true;
                    Debug.Log($"[CodexDataManager] => ClaimRewardForPiece: Not all reward claimed, stay in CodexState.PendingReward");
                    break;
                }
            }

            if (!isAnyRewardRemaining)
            {
                CodexState = CodexState.Normal;
                Debug.Log($"[CodexDataManager] => ClaimRewardForPiece: All reward claimed, set CodexState.Normal");
            }
        }
    }
}