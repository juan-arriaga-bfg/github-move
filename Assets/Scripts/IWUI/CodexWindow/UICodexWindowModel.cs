using System;
using System.Collections.Generic;
using System.Linq;

public class CodexContent
{
    private static int LastInstanceId;
    
    public List<CodexTabDef> TabDefs; 
    public List<CodexChainDef> ChainDefs;
    public List<CodexItemDef> ItemDefs;
    
    public int InstanceId { get; }

    public bool PendingReward
    {
        get
        {
            if (TabDefs == null)
            {
                return false;
            }
            
            for (var i = 0; i < TabDefs.Count; i++)
            {
                if (TabDefs[i].PendingReward)
                {
                    return true;
                }
            }

            return false;
        }
    }

    public List<string> TabHeaders => TabDefs.Select(t => t.Name).ToList();

    public CodexChainDef GetChainDefByFirstItemId(int id)
    {
        if (TabDefs == null)
        {
            return null;
        }
        
        for (var i = 0; i < ChainDefs.Count; i++)
        {
            CodexChainDef chainDef = ChainDefs[i];
            CodexItemDef itemDef = chainDef.ItemDefs[0];
            
            if (itemDef.PieceDef.Id == id)
            {
                return chainDef;
            }
        }

        return null;
    }
    
    public CodexContent()
    {
        LastInstanceId++;
        InstanceId = LastInstanceId;
    }
}

public class UICodexWindowModel : IWWindowModel
{
    public readonly string Title = LocalizationService.Get("window.codex.title", "window.codex.title");

    public int ActiveTabIndex = 0;

    public CodexContent CodexContent;

    public Action OnClose;
}