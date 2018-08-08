using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class CodexItemDef
{
    public PieceTypeDef PieceTypeDef;
    public List<CurrencyPair> PendingReward;
    public bool ShowArrow;
    public CodexItemState State;
    public string Name;
}

public class CodexTabDef
{
    public string Name;
    public List<CodexChainDef> ChainDefs;
}

public class CodexChainDef
{
    public string Name;
    public List<CodexItemDef> ItemDefs;
}

public class CodexContent
{
    public List<CodexTabDef> TabDefs; 
}

public class UICodexWindowModel : IWWindowModel
{
    public readonly string Title = "Codex";
    
    public string Message { get; set; }

    public string AcceptLabel { get; set; }
    public string CancelLabel { get; set; }

    public Action OnReward { get; set; }
    public Action OnClose { get; set; }

    public int ActiveTabIndex = 0;

    public List<CodexItemDef> items;

    public int PendingRewardAmount;

    private List<CodexItemDef> GetCodexItems(List<int> chain)
    {
        Debug.Log($"Get items: {chain[0]}");
        
        List<CodexItemDef> ret = new List<CodexItemDef>();

        var pieceManager = GameDataService.Current.PiecesManager;

        CodexChainState chainState;
        GameDataService.Current.CodexManager.GetChainState(chain[0], out chainState);
        
        for (var i = 0; i < chain.Count; i++)
        {
            int pieceId = chain[i];

            bool isUnlocked = chainState?.Unlocked.Contains(pieceId) ?? false;
            bool isPendingReward = chainState?.PendingReward.Contains(pieceId) ?? false;
            
            PieceDef pieceDef = pieceManager.GetPieceDef(pieceId);
            PieceTypeDef pieceTypeDef = PieceType.GetDefById(pieceId);
            
            // pieceDef.Reproduction // Create by timer
            // pieceDef.SpawnResources // Create on consume 
            
            CodexItemDef itemDef = new CodexItemDef
            {
                PieceTypeDef = pieceTypeDef,
                ShowArrow = i != chain.Count - 1,
                PendingReward = isPendingReward ? pieceDef.UnlockBonus : null,
                Name = pieceDef.Name,
            };
            
            if (isUnlocked)
            {
                itemDef.State = isPendingReward ? CodexItemState.PendingReward : CodexItemState.Unlocked;
            } 
            // else if (i == locked)
            // {
            //     itemDef.State = CodexItemState.PartLock; 
            // } 
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
                else
                {
                    PendingRewardAmount += itemDef.PendingReward[0].Amount;
                }
            }
            
            ret.Add(itemDef);
            items.Add(itemDef);
        }

        return ret;
    }
    
    public CodexContent CodexContent
    {
        get
        {
            items = new List<CodexItemDef>();
            
            var board    = BoardService.Current.GetBoardById(0);
            var matchDef = board.BoardLogic.GetComponent<MatchDefinitionComponent>(MatchDefinitionComponent.ComponentGuid);

            // var ids = PieceType.GetIdsByFilter(PieceTypeFilter.Energy);// Energy (exclude chest)
            // var ids = PieceType.GetIdsByFilter(PieceTypeFilter.Resource);// Coins tab (exclude energy)
            // Buildings - PieceTypeFilter.Simple (from the rest)
            
            CodexContent content = new CodexContent
            {
                TabDefs = new List<CodexTabDef>
                {
                    new CodexTabDef
                    {
                        Name = "Energy",
                        ChainDefs = new List<CodexChainDef>
                        {
                            new CodexChainDef
                            {
                                Name = "Energy 1",
                                ItemDefs = GetCodexItems(matchDef.GetChain(PieceType.D1.Id)),
                            },
                            new CodexChainDef
                            {
                                Name = "Energy 2",
                                ItemDefs = GetCodexItems(matchDef.GetChain(PieceType.E1.Id)),
                            },                            
                            new CodexChainDef
                            {
                                Name = "Energy 3",
                                ItemDefs = GetCodexItems(matchDef.GetChain(PieceType.F1.Id)),
                            },                            
                            new CodexChainDef
                            {
                                Name = "Energy 4",
                                ItemDefs = GetCodexItems(matchDef.GetChain(PieceType.G1.Id)),
                            },                            
                            new CodexChainDef
                            {
                                Name = "Energy 5",
                                ItemDefs = GetCodexItems(matchDef.GetChain(PieceType.H1.Id)),
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
                                ItemDefs = GetCodexItems(matchDef.GetChain(PieceType.A1.Id)),
                            },
                            new CodexChainDef
                            {
                                Name = "Buildings 2",
                                ItemDefs = GetCodexItems(matchDef.GetChain(PieceType.B1.Id))
                            },
                            new CodexChainDef
                            {
                                Name = "Buildings 3",
                                ItemDefs = GetCodexItems(matchDef.GetChain(PieceType.C1.Id))
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
                                ItemDefs = GetCodexItems(matchDef.GetChain(PieceType.Coin1.Id)),
                            },                            
                        }
                    }
                }
            };

            return content;
        }
    }
}