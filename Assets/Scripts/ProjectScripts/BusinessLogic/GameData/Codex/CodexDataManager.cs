// #define FORCE_UNLOCK_ALL

using System.Collections.Generic;
using UnityEngine;

public class CodexDataManager : IECSComponent, IDataManager, IDataLoader<Dictionary<int, CodexChainState>>
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();

    public int Guid => ComponentGuid;

    public Dictionary<int, CodexChainState> Items = new Dictionary<int, CodexChainState>();

    private MatchDefinitionComponent cachedMatchDef;

    private CodexContent codexContentCache = null;
    
    public delegate void NewItemUnlocked();
    public NewItemUnlocked OnNewItemUnlocked;
    
    public CodexState CodexState = CodexState.Normal;
    
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
        
        cachedMatchDef = BoardService.Current?.GetBoardById(0)?.BoardLogic?.GetComponent<MatchDefinitionComponent>(MatchDefinitionComponent.ComponentGuid) ?? new MatchDefinitionComponent(new MatchDefinitionBuilder().Build());

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

        ClearCodexContentCache();

        OnNewItemUnlocked?.Invoke();
        
        GameDataService.Current.QuestsManager.StartNewQuestsIfAny();

        return true;
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
                CodexState = CodexState.NewPiece;
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
            CodexState = CodexState.NewPiece;
        }
    }

    public bool IsHidedFromCodex(int id)
    {
        // Ignore obstacles
        var def = PieceType.GetDefById(id);
        
        if (def.Filter.Has(PieceTypeFilter.Fake))
        {
            return true;
        }
        
        if (def.Filter.Has(PieceTypeFilter.Obstacle))
        {
            return true;
        }
        
        if (def.Filter.Has(PieceTypeFilter.Mine))
        {
            return true;
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

    public List<CodexItemDef> GetCodexItemsForChainAndFocus(int targetId, int length)
    {
        var board    = BoardService.Current.GetBoardById(0);
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
                PendingReward = isPendingReward ? pieceDef.UnlockBonus : null,
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
        
        CodexContent ret = new CodexContent();

        ret.ItemDefs = new List<CodexItemDef>();
            
        var board    = BoardService.Current.GetBoardById(0);
        var matchDef = board.BoardLogic.GetComponent<MatchDefinitionComponent>(MatchDefinitionComponent.ComponentGuid);

        // var ids = PieceType.GetIdsByFilter(PieceTypeFilter.Energy);// Energy (exclude chest)
        // var ids = PieceType.GetIdsByFilter(PieceTypeFilter.Resource);// Coins tab (exclude energy)
        // Buildings - PieceTypeFilter.Simple (from the rest)

        ret.TabDefs = new List<CodexTabDef>
        {
            new CodexTabDef
            {
                Name = LocalizationService.Get("window.codex.toggle.main", "window.codex.toggle.main"),
                ChainDefs = new List<CodexChainDef>
                {
                    new CodexChainDef
                    {
                        Name = LocalizationService.Get("window.codex.branch.A", "window.codex.branch.A"),
                        ItemDefs = GetCodexItemsForChain(matchDef.GetChain(PieceType.A1.Id)),
                    },
                    new CodexChainDef
                    {
                        Name = LocalizationService.Get("window.codex.branch.B", "window.codex.branch.B"),
                        ItemDefs = GetCodexItemsForChain(matchDef.GetChain(PieceType.B1.Id))
                    },
                    new CodexChainDef
                    {
                        Name = LocalizationService.Get("window.codex.branch.C", "window.codex.branch.C"),
                        ItemDefs = GetCodexItemsForChain(matchDef.GetChain(PieceType.C1.Id))
                    },
                    new CodexChainDef
                    {
                        Name = LocalizationService.Get("window.codex.branch.D", "window.codex.branch.D"),
                        ItemDefs = GetCodexItemsForChain(matchDef.GetChain(PieceType.D1.Id))
                    },
                }
            },
            new CodexTabDef
            {
                Name = LocalizationService.Get("window.codex.toggle.production", "window.codex.toggle.production"),
                ChainDefs = new List<CodexChainDef>
                {
                    new CodexChainDef
                    {
                        Name = LocalizationService.Get("window.codex.branch.PR_A", "window.codex.branch.PR_A"),
                        ItemDefs = GetCodexItemsForChain(new List<List<int>>
                        {
                            matchDef.GetChain(PieceType.PR_A1.Id),
                            matchDef.GetChain(PieceType.PR_A5.Id)
                        })
                    },
                    new CodexChainDef
                    {
                        Name = LocalizationService.Get("window.codex.branch.PR_B", "window.codex.branch.PR_B"),
                        ItemDefs = GetCodexItemsForChain(new List<List<int>>
                        {
                            matchDef.GetChain(PieceType.PR_B1.Id),
                            matchDef.GetChain(PieceType.PR_B5.Id)
                        })
                    },
                    new CodexChainDef
                    {
                        Name = LocalizationService.Get("window.codex.branch.PR_C", "window.codex.branch.PR_C"),
                        ItemDefs = GetCodexItemsForChain(new List<List<int>>
                        {
                            matchDef.GetChain(PieceType.PR_C1.Id),
                            matchDef.GetChain(PieceType.PR_C5.Id)
                        })
                    },
                    new CodexChainDef
                    {
                        Name = LocalizationService.Get("window.codex.branch.PR_D", "window.codex.branch.PR_D"),
                        ItemDefs = GetCodexItemsForChain(new List<List<int>>
                        {
                            matchDef.GetChain(PieceType.PR_D1.Id),
                            matchDef.GetChain(PieceType.PR_D5.Id)
                        })
                    },
                    new CodexChainDef
                    {
                        Name = LocalizationService.Get("window.codex.branch.PR_E", "window.codex.branch.PR_E"),
                        ItemDefs = GetCodexItemsForChain(new List<List<int>>
                        {
                            matchDef.GetChain(PieceType.PR_E1.Id),
                            matchDef.GetChain(PieceType.PR_E5.Id)
                        })
                    },
                }
            },
            new CodexTabDef
            {
                Name = LocalizationService.Get("window.codex.toggle.chests", "window.codex.toggle.chests"),
                ChainDefs = new List<CodexChainDef>
                {
                    new CodexChainDef
                    {
                        Name = LocalizationService.Get("window.codex.branch.CH_A", "window.codex.branch.CH_A"),
                        ItemDefs = GetCodexItemsForChain(matchDef.GetChain(PieceType.CH1_A.Id)),
                    },
                    new CodexChainDef
                    {
                        Name = LocalizationService.Get("window.codex.branch.CH_B", "window.codex.branch.CH_B"),
                        ItemDefs = GetCodexItemsForChain(matchDef.GetChain(PieceType.CH1_B.Id)),
                    },
                    new CodexChainDef
                    {
                        Name = LocalizationService.Get("window.codex.branch.CH_C", "window.codex.branch.CH_C"),
                        ItemDefs = GetCodexItemsForChain(matchDef.GetChain(PieceType.CH1_C.Id)),
                    },
                    new CodexChainDef
                    {
                        Name = LocalizationService.Get("window.codex.branch.CH_D", "window.codex.branch.CH_D"),
                        ItemDefs = GetCodexItemsForChain(matchDef.GetChain(PieceType.CH1_D.Id)),
                    },
                }
            },
            new CodexTabDef
            {
                Name = LocalizationService.Get("window.codex.toggle.currency", "window.codex.toggle.currency"),
                ChainDefs = new List<CodexChainDef>
                {
                    new CodexChainDef
                    {
                        Name = LocalizationService.Get("window.codex.branch.Soft", "window.codex.branch.Soft"),
                        ItemDefs = GetCodexItemsForChain(matchDef.GetChain(PieceType.Soft1.Id)),
                    },
                    new CodexChainDef
                    {
                        Name = LocalizationService.Get("window.codex.branch.Hard", "window.codex.branch.Hard"),
                        ItemDefs = GetCodexItemsForChain(matchDef.GetChain(PieceType.Hard1.Id)),
                    },
                }
            },
            new CodexTabDef
            {
                Name = LocalizationService.Get("window.codex.toggle.boosters", "window.codex.toggle.boosters"),
                ChainDefs = new List<CodexChainDef>
                {
                    new CodexChainDef
                    {
                        Name = LocalizationService.Get("window.codex.branch.Boost_WR", "window.codex.branch.Boost_WR"),
                        ItemDefs = GetCodexItemsForChain(matchDef.GetChain(PieceType.Boost_WR.Id)),
                    },
                    new CodexChainDef
                    {
                        Name = LocalizationService.Get("window.codex.branch.Boost_CR", "window.codex.branch.Boost_CR"),
                        ItemDefs = GetCodexItemsForChain(matchDef.GetChain(PieceType.Boost_CR1.Id)),
                    },
                }
            }
        };

        // todo: optimize
        foreach (var tabDef in ret.TabDefs)
        {
            foreach (var chainDef in tabDef.ChainDefs)
            {
                ret.ItemDefs.AddRange(chainDef.ItemDefs);
                foreach (var itemDef in chainDef.ItemDefs)
                {
                    int amount = itemDef.PendingReward?[0].Amount ?? 0;
                    if (amount > 0)
                    {
                        ret.PendingRewardAmount += amount;
                        tabDef.PendingReward = true;
                    }

                    if (amount <= 0 && itemDef.State == CodexItemState.PendingReward)
                    {
                        itemDef.State = CodexItemState.Unlocked;
                        Debug.LogError($"[CodexDataManager] => No reward specified for item {itemDef.PieceTypeDef.Abbreviations[0]}");
                    }
                }
            }
        }

        codexContentCache = ret;
        return codexContentCache;
    }

    public void ClearCodexContentCache()
    {
        codexContentCache = null;
    }
}