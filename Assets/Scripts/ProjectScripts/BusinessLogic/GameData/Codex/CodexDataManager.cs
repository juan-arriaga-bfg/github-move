// #define FORCE_UNLOCK_ALL

using System.Collections.Generic;
using UnityEngine;

public class CodexDataManager : IECSComponent, IDataManager, IDataLoader<Dictionary<int, CodexChainState>>
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();

    public int Guid => ComponentGuid;

    public Dictionary<int, CodexChainState> Items = new Dictionary<int, CodexChainState>();

    private MatchDefinitionComponent matchDef;

    private CodexContent codexContentCache = null;
    
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
        ClearCodexContentCache();

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

        ClearCodexContentCache();
        
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

    private List<CodexItemDef> GetCodexItemsForChain(List<int> chain)
    {
        // Debug.Log($"Get items: {chain[0]}");
        
        List<CodexItemDef> ret = new List<CodexItemDef>();

        var pieceManager = GameDataService.Current.PiecesManager;

        CodexChainState chainState;
        GameDataService.Current.CodexManager.GetChainState(chain[0], out chainState);

        bool isPreviousPieceUnlocked = false;
        
        for (var i = 0; i < chain.Count; i++)
        {
            int pieceId = chain[i];

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
                        Name = "Energy 1",
                        ItemDefs = GetCodexItemsForChain(matchDef.GetChain(PieceType.D1.Id)),
                    },
                    new CodexChainDef
                    {
                        Name = "Energy 2",
                        ItemDefs = GetCodexItemsForChain(matchDef.GetChain(PieceType.E1.Id)),
                    },
                    new CodexChainDef
                    {
                        Name = "Energy 3",
                        ItemDefs = GetCodexItemsForChain(matchDef.GetChain(PieceType.F1.Id)),
                    },
                    new CodexChainDef
                    {
                        Name = "Energy 4",
                        ItemDefs = GetCodexItemsForChain(matchDef.GetChain(PieceType.G1.Id)),
                    },
                    new CodexChainDef
                    {
                        Name = "Energy 5",
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
                        Name = "Buildings 1",
                        ItemDefs = GetCodexItemsForChain(matchDef.GetChain(PieceType.A1.Id)),
                    },
                    new CodexChainDef
                    {
                        Name = "Buildings 2",
                        ItemDefs = GetCodexItemsForChain(matchDef.GetChain(PieceType.B1.Id))
                    },
                    new CodexChainDef
                    {
                        Name = "Buildings 3",
                        ItemDefs = GetCodexItemsForChain(matchDef.GetChain(PieceType.C1.Id))
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
                        Name = "Coins 1",
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