using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CodexItem
{
    public PieceTypeDef PieceTypeDef;
    public bool Unlocked;
    public CurrencyPair PendingReward;
}

public class CodexTab
{
    public string Name;
    public List<CodexChain> CodexChains;
}

public class CodexChain
{
    public string Name;
    public List<CodexItem> CodexItems; 
}

public class CodexContent
{
    public List<CodexTab> CodexTabs; 
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

    private List<CodexItem> GetCodexItems(List<int> chain)
    {
        List<CodexItem> ret = new List<CodexItem>();

        foreach (var i in chain)
        {
            CodexItem item = new CodexItem
            {
                PieceTypeDef = PieceType.GetDefById(chain[i]),
            };
            
            ret.Add(item);
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
                CodexTabs = new List<CodexTab>
                {
                    new CodexTab
                    {
                        Name = "Energy",
                        CodexChains = new List<CodexChain>
                        {
                            new CodexChain
                            {
                                Name = "Apple",
                                CodexItems = GetCodexItems(matchDef.GetChain(PieceType.A1.Id))
                            },
                            new CodexChain
                            {
                                Name = "Corn",
                                CodexItems = GetCodexItems(matchDef.GetChain(PieceType.B1.Id))
                            },
                        }
                    },
                    new CodexTab
                    {
                        Name = "Buildings",
                        CodexChains = new List<CodexChain>
                        {
                            new CodexChain
                            {
                                Name = "BUilding1",
                                CodexItems = GetCodexItems(matchDef.GetChain(PieceType.A1.Id))
                            },
                            new CodexChain
                            {
                                Name = "Building2",
                                CodexItems = GetCodexItems(matchDef.GetChain(PieceType.B1.Id))
                            },
                        }
                    }
                }
            };

            return content;
        }
    }
}