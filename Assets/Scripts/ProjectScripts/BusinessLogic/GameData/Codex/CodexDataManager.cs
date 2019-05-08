// #define FORCE_UNLOCK_ALL
using Debug = IW.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using BfgAnalytics;
using UnityEngine;

public partial class CodexDataManager : IECSComponent, IDataManager, IDataLoader<Dictionary<int, CodexChainState>>
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();

    public int Guid => ComponentGuid;

    public Dictionary<int, CodexChainState> Items = new Dictionary<int, CodexChainState>();

    private CodexContent codexContentCache = null;
    
    public Action OnNewItemUnlocked;
    
    public Action<int> OnPieceRewardClaimed;
    
    public CodexState CodexState { get; private set; }= CodexState.Normal;
    
    private ECSEntity context;
    
    public readonly List<PieceTypeFilter> hidedPieceFilters = new List<PieceTypeFilter>
    {
        PieceTypeFilter.Fake,
        PieceTypeFilter.Obstacle,
        PieceTypeFilter.Mine,
        PieceTypeFilter.OrderPiece,
    }; 
        
    public readonly HashSet<int> hidedPieceIds = new HashSet<int>
    {
        PieceType.Fog.Id,
        PieceType.LockedEmpty.Id,
        PieceType.CH_Free.Id,
        PieceType.CH_NPC.Id,
        PieceType.NPC_SleepingBeautyPlaid.Id,
        PieceType.NPC_Gnome.Id,
        PieceType.Token1.Id,
        PieceType.Token2.Id,
        PieceType.Token3.Id,
    }; 
    
    public const int CHAR_CHAIN_VISIBLE_COUNT = 6; // Size of char parts' chain in the codex dialog
    public const int CHAR_CHAIN_VISIBLE_ITEMS_AFTER_LAST_UNLOCKED = 2;
    
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
                var save = ((GameDataManager)context).UserProfile.GetComponent<CodexSaveComponent>(CodexSaveComponent.ComponentGuid);
                if (save?.Data == null || save.Data.Count == 0)
                {
                    Items = data;

                    // Unlock all pieces on game field...
                    UnlockPiece(PieceType.Empty.Id);
                    UnlockPiece(PieceType.None.Id);
                    
                    var pieces = GameDataService.Current.FieldManager.BoardPieces;
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
        Debug.Log($"[CodexDataManager] => UnlockPiece: id: {id} - {PieceType.Parse(id)}");
        
        int firstInChain = GameDataService.Current.MatchDefinition.GetFirst(id);

        // Debug.Log($"UnlockPiece: first in chain for {id} is {firstInChain}");

        if (Items.TryGetValue(firstInChain, out var state))
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

        HandleCharPieceUnlock(id);
        HandleCharPartUnlock(id);
    }

    /// <summary>
    /// We should remove reward from all char parts pieces which doesn't fit into codex window
    /// </summary>
    /// <param name="id"></param>
    private void HandleCharPartUnlock(int id)
    {
        List<int> chain = GameDataService.Current.MatchDefinition.GetChain(id);
        if (chain == null || chain.Count < 2)
        {
            return;
        }
        
        var lastId = chain[chain.Count - 1];
        if (!PieceType.GetDefById(lastId).Filter.Has(PieceTypeFilter.Character))
        {
            return;
        }
        
        int startIndex = GameDataService.Current.CodexManager.GetCharChainStartIndex(lastId);
        if (Items.TryGetValue(chain[0], out var chainState))
        {                                                                               // -2 here to handle char piece removed from the end of chain
            for (int i = 0; i < startIndex && i + CHAR_CHAIN_VISIBLE_COUNT <= chain.Count - 2; i++)
            {
                int idToClear = chain[i];
                if (chainState.PendingReward.Contains(idToClear))
                {
                    Debug.Log($"[CodexDataManager] => HandleCharPartUnlock({PieceType.Parse(id)}: StartIndex: {startIndex}: Clear reward for {PieceType.Parse(idToClear)})");
                    
                    chainState.PendingReward.Remove(idToClear);
                }
            }
        }
    }

    private void HandleCharPieceUnlock(int id)
    {
        if (!PieceType.GetDefById(id).Filter.Has(PieceTypeFilter.Character))
        {
            return;
        }
        
        var charChain = GameDataService.Current.MatchDefinition.GetChain(id);
        if (Items.TryGetValue(charChain[0], out var chainState))
        {
            bool isChanged = false;
            foreach (var pieceId in charChain)
            {
                if (pieceId != id)
                {
                    if (chainState.PendingReward.Remove(pieceId))
                    {
                        isChanged = true;
                    }
                }
            }

            if (isChanged)
            {
                ValidateCodexState();
                ClearCodexContentCache();
            }
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

        var firstInChain = ((GameDataManager)context).MatchDefinition.GetFirst(id);
        
        return Items.TryGetValue(firstInChain, out var state) && state.Unlocked.Contains(id);
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

    public List<CodexItemDef> GetCodexItemsForChainAndFocus(int targetId, int length, bool hideCaptions, bool allowRewards, bool allowHighlight)
    {
        var chain = GameDataService.Current.MatchDefinition.GetChain(targetId);

        // Current piece is not a part of any chain
        if (chain.Count == 0)
        {
            return null;
        }
        
        var list = GetCustomChain(targetId, hideCaptions, allowRewards, allowHighlight, chain, out var highlightedIndex);

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
        
        EnsureThatNoLockedItemsBetweenUnlocked(ref list);
        
        List<CodexItemDef> ret = list.GetRange(rangeStart, rangeLength);
        return ret;
    }

    /// <summary>
    /// targetId - to get chain and highlight
    /// </summary>
    public List<CodexItemDef> GetCodexItemsForChainStartingFrom(int targetId, int skipFromStart, int skipFromEnd, int length, bool hideCaptions, bool allowRewards, bool allowHighlight)
    {
        var chain = GameDataService.Current.MatchDefinition.GetChain(targetId);

        // Current piece is not a part of any chain
        if (chain.Count == 0)
        {
            return null;
        }
        
        var list = GetCustomChain(targetId, hideCaptions, allowRewards, allowHighlight, chain, out _);
        for (int i = 1; i <= skipFromEnd; i++)
        {
            list.RemoveAt(list.Count - 1);
        }

        if (list.Count < length)
        {
            return list;
        }
        
        int rangeStart;
        int rangeLength;
        if (list.Count - skipFromStart >= length)
        {
            rangeStart = skipFromStart;
            rangeLength = length;
        }
        else
        {
            rangeStart  = list.Count - length;
            rangeLength = length;
        }
        
        List<CodexItemDef> ret = list.GetRange(rangeStart, rangeLength);
        return ret;
    }
    
    private List<CodexItemDef> GetCustomChain(int targetId, bool hideCaptions, bool allowRewards, bool allowHighlight, List<int> chain, out int highlightedIndex)
    {
        var list = GetCodexItemsForChain(chain);

        GameDataService.Current.CodexManager.GetChainState(chain[0], out var chainState);

        highlightedIndex = -1;

        bool isPreviousPieceUnlocked = false;
        
        for (var i = 0; i < list.Count; i++)
        {
            var def = list[i];

            bool isUnlocked = chainState?.Unlocked.Contains(def.PieceDef.Id) ?? false;

            if (!isUnlocked)
            {
                def.State = isPreviousPieceUnlocked
                    ? CodexItemState.PartLock
                    : CodexItemState.FullLock;
            }

            // To enable CodexItemState.PartLock for the next item
            isPreviousPieceUnlocked = isUnlocked;
            
            if (!allowRewards && def.State == CodexItemState.PendingReward)
            {
                def.State = CodexItemState.Unlocked;
            }

            if (allowHighlight && def.PieceTypeDef.Id == targetId)
            {
                if (def.State == CodexItemState.Unlocked)
                {
                    def.State = CodexItemState.Highlighted;
                }

                highlightedIndex = i;
            }

            def.HideCaption = hideCaptions;
        }

        return list;
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


    public static bool IsChainContainsOnlyChars(List<int> chain)
    {
        bool ret = true;
        foreach (var id in chain)
        {
            if (!PieceType.GetDefById(id).Filter.Has(PieceTypeFilter.Character))
            {
                ret = false;
                break;
            }
        }
        
        return ret; 
    }
    
    public bool IsAnyPieceUnlockedInChain(List<int> chain)
    {
        bool anyItemUnlocked = false;
        foreach (var id in chain)
        {
            if (IsPieceUnlocked(id))
            {
                anyItemUnlocked = true;
                break;
            }
        }

        return anyItemUnlocked;
    }

    public bool IsAnyPendingRewardInChain(List<int> chain)
    {
        if (chain.Count == 0)
        {
            return false;
        }
        
        GetChainState(chain[0], out var chainState);
        foreach (var id in chain)
        {
            bool isPendingReward = chainState?.PendingReward.Contains(id) ?? false;
            if (isPendingReward)
            {
                return true;
            }
        }

        return false;
    }

    public bool IsAnyPendingRewardForCharChain(int charId)
    {
        var charChain = GameDataService.Current.MatchDefinition.GetChain(charId);
        
        // Do not count char's piece reward because we have dedicated slot for it
        //charChain.Remove(charId);
        
        var isAnyPendingReward = IsAnyPendingRewardInChain(charChain);

        return isAnyPendingReward;
    }
    
    public List<CodexItemDef> GetCodexItemsForChain(List<int> chain)
    {
        // Debug.Log($"========\nGet items: {chain[0]}");
        
        List<CodexItemDef> ret = new List<CodexItemDef>();

        var pieceManager = GameDataService.Current.PiecesManager;
        var matchDef = GameDataService.Current.MatchDefinition;
        
        bool isCharsOnlyChain = IsChainContainsOnlyChars(chain);

        CodexChainState chainState = null;
        
        if (!isCharsOnlyChain)
        {
            GameDataService.Current.CodexManager.GetChainState(chain[0], out chainState);
        }

        bool isPreviousPieceUnlocked = false;
        
        for (var i = 0; i < chain.Count; i++)
        {
            int pieceId = chain[i];

            if (isCharsOnlyChain)
            {
                int pieceToFindChainState = matchDef.GetFirst(pieceId);
                GameDataService.Current.CodexManager.GetChainState(pieceToFindChainState, out chainState);
            }
            
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
            else if (isPreviousPieceUnlocked && !isCharsOnlyChain)
            {
                itemDef.State = CodexItemState.PartLock; 
            } 
            else
            {
                itemDef.State = CodexItemState.FullLock; 
            }

            // Hack for chars
            if (isCharsOnlyChain && itemDef.State == CodexItemState.FullLock)
            {
                var charChain = GameDataService.Current.MatchDefinition.GetChain(pieceDef.Id);
                var isAnyUnlocked = IsAnyPieceUnlockedInChain(charChain);
                if (isAnyUnlocked)
                {
                    itemDef.State = CodexItemState.PartLock; 
                }
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
        
        EnsureThatNoLockedItemsBetweenUnlocked(ref ret);

        return ret;
    }

    private static void EnsureThatNoLockedItemsBetweenUnlocked(ref List<CodexItemDef> items)
    {
        // Ensure that no locked items between unlocked
        bool unlockedFound = false;
        for (int i = items.Count - 1; i >= 0; i--)
        {
            var item = items[i];
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
        var chainId = GameDataService.Current.MatchDefinition.GetFirst(pieceId);

        if (Items.TryGetValue(chainId, out var chainState))
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
                    // But char pieces are always allowed
                    var lastItemInChain = GameDataService.Current.MatchDefinition.GetLast(item.Key);
                    if (!PieceType.GetDefById(lastItemInChain).Filter.Has(PieceTypeFilter.Character))
                    {
                        continue;
                    }
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
    
    public int GetCharChainStartIndex(int id)
    {
        var chain = GameDataService.Current.MatchDefinition.GetChain(id);
        chain.RemoveAt(chain.Count - 1); // Do not count char piece
        
        int chainLen = chain.Count;
        if (chainLen <= CHAR_CHAIN_VISIBLE_COUNT)
        {
            return 0;
        }

        var codexManager = GameDataService.Current.CodexManager;

        int lastUnlocked = 0;
        for (int i = chain.Count - 1; i >= 0; i--)
        {
            int item = chain[i];
            if (codexManager.IsPieceUnlocked(item))
            {
                lastUnlocked = i;
                break;
            }
        }

        int ret = lastUnlocked + CHAR_CHAIN_VISIBLE_ITEMS_AFTER_LAST_UNLOCKED - CHAR_CHAIN_VISIBLE_COUNT + 1;

        if (ret < 0)
        {
            ret = 0;
        }
        
        return ret;
    }
}