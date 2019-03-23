using DG.Tweening;
using UnityEngine;

public class CodexCharItem : CodexItem
{
    [IWUIBinding("#ExclamationMark")] private GameObject exclamationMark;
    
    private Vector3 selectedScale = new Vector3(1.2f, 1.2f, 1.2f);
    
    private Vector3 normalScale = new Vector3(1f, 1f, 1f);
    
    private const float ScaleDuration = 0.25f;
    
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
            codexTab.SelectCodexItem(this);
        }
        
        base.OnClick();
    }

    public override void OnSelect(CodexItem selectedItem)
    {
        base.OnSelect(selectedItem);
        
        if (selectedItem == this)
        {
            SetStateSelected();
            
            DOTween.Kill(this);
            var sequence = DOTween.Sequence().SetId(this);
            sequence.Insert(0f, CachedTransform.DOScale(selectedScale, ScaleDuration));
            sequence.Insert(0f, selectedMaterial.DOFloat(0f, "_WhiteOverlayCoef", ScaleDuration));
        }
        else
        {
            OnDeselect(selectedItem);
        }
    }

    public override void OnDeselect(CodexItem selectedItem)
    {
        base.OnDeselect(selectedItem);
        
        if (selectedItem == this) return;
        
        SetStateSelected();
        
        DOTween.Kill(this);
        var sequence = DOTween.Sequence().SetId(this);
        sequence.Insert(0f, CachedTransform.DOScale(normalScale, ScaleDuration));
        sequence.Insert(0f, selectedMaterial.DOFloat(0.2f, "_WhiteOverlayCoef", ScaleDuration));
    }

    public void ToggleExclamationMark(bool isPendingReward)
    {
        exclamationMark.SetActive(isPendingReward);
    }
}