using UnityEngine;

public class CodexCharItem : CodexItem
{
    [IWUIBinding("#ExclamationMark")] protected GameObject exclamationMark;

    public override void Setup(CodexItemDef itemDef, bool forceHideArrow)
    {
        base.Setup(itemDef, forceHideArrow);

        var isAnyPendingReward = GameDataService.Current.CodexManager.IsAnyPendingRewardForCharChain(itemDef.PieceDef.Id);
        exclamationMark.SetActive(isAnyPendingReward);
    }

    public override void OnClick()
    {
        if (Context == null)
        {
            return;
        }

        CodexTab codexTab = Context.Context;
        if (codexTab != null && codexTab.IsHero)
        {
            codexTab.SelectItem.SetItem(def.PieceDef, def.State);
        }
        
        base.OnClick();
    }
}