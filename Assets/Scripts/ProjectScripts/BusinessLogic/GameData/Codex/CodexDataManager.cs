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
        
        if (def.Filter.Has(PieceTypeFilter.Obstacle))
        {
            return true;
        }
        
        if (def.Filter.Has(PieceTypeFilter.Chest))
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

    private List<CodexItemDef> GetCodexItemsForChain(List<int> chain)
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
            else if (isPreviousPieceUnlocked)
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
                Name = "Energy",
                ChainDefs = new List<CodexChainDef>
                {
                    new CodexChainDef
                    {
                        Name = "Corn",
                        ItemDefs = GetCodexItemsForChain(matchDef.GetChain(PieceType.F1.Id)),
                    },
                    new CodexChainDef
                    {
                        Name = "Wool",
                        ItemDefs = GetCodexItemsForChain(matchDef.GetChain(PieceType.D1.Id)),
                    },
                    new CodexChainDef
                    {
                        Name = "Apple",
                        ItemDefs = GetCodexItemsForChain(matchDef.GetChain(PieceType.E1.Id)),
                    },
                    new CodexChainDef
                    {
                        Name = "Milk",
                        ItemDefs = GetCodexItemsForChain(matchDef.GetChain(PieceType.G1.Id)),
                    },
                    new CodexChainDef
                    {
                        Name = "Egg",
                        ItemDefs = GetCodexItemsForChain(matchDef.GetChain(PieceType.H1.Id)),
                    },
                }
            },
            new CodexTabDef
            {
                Name = "Buildings",
                ChainDefs = new List<CodexChainDef>
                {
                    new CodexChainDef
                    {
                        Name = "Wood",
                        ItemDefs = GetCodexItemsForChain(matchDef.GetChain(PieceType.A1.Id)),
                    },
                    new CodexChainDef
                    {
                        Name = "Farm",
                        ItemDefs = GetCodexItemsForChain(matchDef.GetChain(PieceType.B1.Id))
                    },
                    new CodexChainDef
                    {
                        Name = "Stone",
                        ItemDefs = GetCodexItemsForChain(matchDef.GetChain(PieceType.C1.Id))
                    },
                }
            },
            new CodexTabDef
            {
                Name = "Chests",
                ChainDefs = new List<CodexChainDef>
                {
                    new CodexChainDef
                    {
                        Name = "Stone",
                        ItemDefs = GetCodexItemsForChain(matchDef.GetChain(PieceType.ChestC1.Id)),
                    },                         
                    // new CodexChainDef
                    // {
                    //     Name = "Chests",
                    //     ItemDefs = GetCodexItemsForChain(matchDef.GetChain(PieceType.Chest1.Id)),
                    // },                    
                    new CodexChainDef
                    {
                        Name = "Food",
                        ItemDefs = GetCodexItemsForChain(matchDef.GetChain(PieceType.Basket1.Id)),
                    },
                    
                }
            },
            new CodexTabDef
            {
                Name = "Coins",
                ChainDefs = new List<CodexChainDef>
                {
                    new CodexChainDef
                    {
                        Name = "Coin",
                        ItemDefs = GetCodexItemsForChain(matchDef.GetChain(PieceType.Coin1.Id)),
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