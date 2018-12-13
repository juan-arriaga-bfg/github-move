using System.Collections.Generic;
using UnityEngine;

public partial class CodexDataManager
{
    private CodexContent BuildContent()
    {
        CodexContent ret = new CodexContent();

        ret.ItemDefs = new List<CodexItemDef>();

        var board = BoardService.Current.GetBoardById(0);
        var matchDef = board.BoardLogic.GetComponent<MatchDefinitionComponent>(MatchDefinitionComponent.ComponentGuid);

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
                            matchDef.GetChain(PieceType.PR_A1.Id)
                        })
                    },
                    new CodexChainDef
                    {
                        Name = LocalizationService.Get("window.codex.branch.PR_B", "window.codex.branch.PR_B"),
                        ItemDefs = GetCodexItemsForChain(new List<List<int>>
                        {
                            matchDef.GetChain(PieceType.PR_B1.Id)
                        })
                    },
                    new CodexChainDef
                    {
                        Name = LocalizationService.Get("window.codex.branch.PR_C", "window.codex.branch.PR_C"),
                        ItemDefs = GetCodexItemsForChain(new List<List<int>>
                        {
                            matchDef.GetChain(PieceType.PR_C1.Id)
                        })
                    },
                    new CodexChainDef
                    {
                        Name = LocalizationService.Get("window.codex.branch.PR_D", "window.codex.branch.PR_D"),
                        ItemDefs = GetCodexItemsForChain(new List<List<int>>
                        {
                            matchDef.GetChain(PieceType.PR_D1.Id)
                        })
                    },
                    new CodexChainDef
                    {
                        Name = LocalizationService.Get("window.codex.branch.PR_E", "window.codex.branch.PR_E"),
                        ItemDefs = GetCodexItemsForChain(new List<List<int>>
                        {
                            matchDef.GetChain(PieceType.PR_E1.Id)
                        })
                    },
                    new CodexChainDef
                    {
                        Name = LocalizationService.Get("window.codex.branch.PR_F", "window.codex.branch.PR_F"),
                        ItemDefs = GetCodexItemsForChain(new List<List<int>>
                        {
                            matchDef.GetChain(PieceType.PR_F1.Id)
                        })
                    },
                    new CodexChainDef
                    {
                        Name = LocalizationService.Get("window.codex.branch.PR_G", "window.codex.branch.PR_G"),
                        ItemDefs = GetCodexItemsForChain(new List<List<int>>
                        {
                            matchDef.GetChain(PieceType.PR_G1.Id)
                        })
                    },
                }
            },
            new CodexTabDef
            {
                Name = LocalizationService.Get("window.codex.toggle.chests", "window.codex.toggle.chests"),
                ChainDefs = new List<CodexChainDef>
                {
                    // new CodexChainDef
                    // {
                    //     Name = LocalizationService.Get("window.codex.branch.SK_PR", "window.codex.branch.SK_PR"),
                    //     ItemDefs = GetCodexItemsForChain(matchDef.GetChain(PieceType.SK1_PR.Id)),
                    // },
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
                    // new CodexChainDef
                    // {
                    //     Name = LocalizationService.Get("window.codex.branch.CH_C", "window.codex.branch.CH_C"),
                    //     ItemDefs = GetCodexItemsForChain(matchDef.GetChain(PieceType.CH1_C.Id)),
                    // },
                    // new CodexChainDef
                    // {
                    //     Name = LocalizationService.Get("window.codex.branch.CH_D", "window.codex.branch.CH_D"),
                    //     ItemDefs = GetCodexItemsForChain(matchDef.GetChain(PieceType.CH1_D.Id)),
                    // },
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
                    // new CodexChainDef
                    // {
                    //     Name = LocalizationService.Get("window.codex.branch.Boost_WR", "window.codex.branch.Boost_WR"),
                    //     ItemDefs = GetCodexItemsForChain(matchDef.GetChain(PieceType.Boost_WR.Id)),
                    // },
                    new CodexChainDef
                    {
                        Name = LocalizationService.Get("window.codex.branch.Boost_CR", "window.codex.branch.Boost_CR"),
                        ItemDefs = GetCodexItemsForChain(matchDef.GetChain(PieceType.Boost_CR1.Id)),
                    },
                    new CodexChainDef
                    {
                        Name = LocalizationService.Get("window.codex.branch.SK_PR", "window.codex.branch.SK_PR"),
                        ItemDefs = GetCodexItemsForChain(matchDef.GetChain(PieceType.SK1_PR.Id)),
                    },
                }
            }
        };

        ret.ChainDefs = new List<CodexChainDef>();
        
        foreach (var tabDef in ret.TabDefs)
        {
            foreach (var chainDef in tabDef.ChainDefs)
            {
                ret.ChainDefs.Add(chainDef);
                ret.ItemDefs.AddRange(chainDef.ItemDefs);
                for (var i = 0; i < chainDef.ItemDefs.Count; i++)
                {
                    var itemDef = chainDef.ItemDefs[i];
                    int amount = itemDef.PendingReward?[0].Amount ?? 0;
                    if (amount <= 0 && itemDef.State == CodexItemState.PendingReward)
                    {
                        itemDef.State = CodexItemState.Unlocked;
                        Debug.LogError($"[CodexDataManager] => No reward specified for item {itemDef.PieceTypeDef.Abbreviations[0]}");
                    }
                }
            }
        }

        return ret;
    }

}