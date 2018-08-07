using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class CodexItemDef
{
    public PieceTypeDef PieceTypeDef;
    public bool Unlocked;
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

    public int ActiveTabIndex { get; set; } = 0;

    public List<CodexItemDef> items;

    private List<CodexItemDef> GetCodexItems(List<int> chain)
    {
        List<CodexItemDef> ret = new List<CodexItemDef>();

        int locked = Random.Range(0, chain.Count);

        var pieceManager = GameDataService.Current.PiecesManager;
        
        for (var i = 0; i < chain.Count; i++)
        {
            int pieceId = chain[i];
            
            PieceDef pieceDef = pieceManager.GetPieceDef(pieceId);
            PieceTypeDef pieceTypeDef = PieceType.GetDefById(pieceId);
            
            CodexItemDef itemDef = new CodexItemDef
            {
                PieceTypeDef = pieceTypeDef,
                ShowArrow = i != chain.Count - 1,
                PendingReward = pieceDef.UnlockBonus,
                Name = pieceDef.Name
            };

            if (i < locked)
            {
                itemDef.State = itemDef.PendingReward == null ? CodexItemState.PendingReward : CodexItemState.Unlocked;
            } 
            else if (i == locked)
            {
                itemDef.State = CodexItemState.PartLock; 
            } 
            else if (i > locked)
            {
                itemDef.State = CodexItemState.FullLock; 
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
            items.Clear();
            
            var board    = BoardService.Current.GetBoardById(0);
            var matchDef = board.BoardLogic.GetComponent<MatchDefinitionComponent>(MatchDefinitionComponent.ComponentGuid);

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
                                Name = "Chain 1",
                                ItemDefs = GetCodexItems(matchDef.GetChain(PieceType.A1.Id))
                            },
                            new CodexChainDef
                            {
                                Name = "Chain 2",
                                ItemDefs = GetCodexItems(matchDef.GetChain(PieceType.B1.Id))
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
                                Name = "BUilding1",
                                ItemDefs = GetCodexItems(matchDef.GetChain(PieceType.C1.Id))
                            },
                            new CodexChainDef
                            {
                                Name = "Building2",
                                ItemDefs = GetCodexItems(matchDef.GetChain(PieceType.D1.Id))
                            },
                            new CodexChainDef
                            {
                                Name = "Building3",
                                ItemDefs = GetCodexItems(matchDef.GetChain(PieceType.E1.Id))
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
                                ItemDefs = GetCodexItems(matchDef.GetChain(PieceType.Coin1.Id))
                            }
                        }
                    }
                }
            };

            return content;
        }
    }
}