using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UIOrdersTabButtonViewController : UIBaseButtonViewController
{
    [IWUIBindingNullable] private CanvasGroup group;
    [IWUIBindingNullable("#Checkmark")] private Image checkmark;
    
    private float scale = 1.1f;

    public UIButtonViewController SetActiveScale(float value)
    {
        scale = value;
        return this;
    }
    
    public bool Interactable
    {
        get { return group == null || group.interactable; }
        set { if (group != null) group.interactable = value; }
    }
    
    public override void UpdateView()
    {
        base.UpdateView();
        
        var value = state == GenericButtonState.Active ? scale : 1f;
        var time = 0.1f * CachedRectTransform.localScale.x / value;
        
        DOTween.Kill(CachedRectTransform);
        CachedRectTransform.DOScale(value, time).SetId(CachedRectTransform);
        
        if(checkmark == null) return;
        
        var alpha = state == GenericButtonState.Active ? 1f : 0f;
        
        DOTween.Kill(checkmark);
        checkmark.DOFade(alpha, time).SetId(checkmark);
    }

    protected override void AnimateOnPointerDown()
    {
    }

    protected override void AnimateOnPointerUp()
    {
    }

    protected override void AnimateOnPointerClick()
    {
    }
}