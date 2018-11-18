using System;
using System.Collections.Generic;

public class CodexItemDef
{
    public PieceTypeDef PieceTypeDef;
    public List<CurrencyPair> PendingReward;
    public bool ShowArrow;
    public CodexItemState State;
    public PieceDef PieceDef;
}

public class CodexTabDef
{
    public string Name;
    public List<CodexChainDef> ChainDefs;
    public bool PendingReward;
}

public class CodexChainDef
{
    public string Name;
    public List<CodexItemDef> ItemDefs;
}

public class CodexContent
{
    private static int LastInstanceId;
    
    public List<CodexItemDef> ItemDefs;
    public List<CodexTabDef> TabDefs; 
    public int PendingRewardAmount;
    public int InstanceId { get; }
    
    public CodexContent()
    {
        LastInstanceId++;
        InstanceId = LastInstanceId;
    }
}

public class UICodexWindowModel : IWWindowModel
{
    public readonly string Title = LocalizationService.Instance.Manager.GetTextByUid("window.codex.title", "Codex");

    public int ActiveTabIndex = 0;

    public CodexContent CodexContent;

    public Action OnClaim;
}