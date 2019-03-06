using UnityEngine;

public class CodexCharItem : CodexItem
{
    [IWUIBinding("#ExclamationMark")] private GameObject exclamationMark;

    public override void Setup(CodexItemDef itemDef, bool forceHideArrow)
    {
        base.Setup(itemDef, forceHideArrow);

        var isPendingReward = GameDataService.Current.CodexManager.IsAnyPendingRewardForCharChain(itemDef.PieceDef.Id);
        ToggleExclamationMark(isPendingReward);
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

    public void ToggleExclamationMark(bool isPendingReward)
    {
        exclamationMark.SetActive(isPendingReward);
    }
}