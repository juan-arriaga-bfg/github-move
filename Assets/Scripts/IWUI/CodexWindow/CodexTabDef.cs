using System.Collections.Generic;

public class CodexTabDef
{
    public string Name;
    public List<CodexChainDef> ChainDefs;

    public bool PendingReward
    {
        get
        {
            if (ChainDefs == null)
            {
                return false;
            }

            for (var chainIndex = 0; chainIndex < ChainDefs.Count; chainIndex++)
            {
                var chainDef = ChainDefs[chainIndex];
                for (var itemIndex = 0; itemIndex < chainDef.ItemDefs.Count; itemIndex++)
                {
                    var item = chainDef.ItemDefs[itemIndex];
                    if (item.State == CodexItemState.PendingReward)
                    {
                        return true;
                    }
                    
                    if (chainDef.IsHero)
                    {
                        if (GameDataService.Current.CodexManager.IsAnyPendingRewardForCharChain(item.PieceDef.Id))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }
    }
}