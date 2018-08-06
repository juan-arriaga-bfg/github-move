using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CodexItemDef
{
    public PieceTypeDef PieceTypeDef;
    public bool Unlocked;
    public CurrencyPair PendingReward;
    public bool IsLast;
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

    private List<CodexItemDef> GetCodexItems(List<int> chain)
    {
        List<CodexItemDef> ret = new List<CodexItemDef>();

        for (var i = 0; i < chain.Count; i++)
        {
            CodexItemDef itemDef = new CodexItemDef
            {
                PieceTypeDef = PieceType.GetDefById(chain[i]),
                IsLast = i == chain.Count - 1
            };

            ret.Add(itemDef);
        }

        return ret;
    }
    
    public CodexContent CodexContent
    {
        get
        {
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