using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UIOrdersTabButtonViewController : UIBaseButtonViewController
{
    [IWUIBinding] private Transform body;
    [IWUIBinding] private CanvasGroup group;
    [IWUIBindingNullable("#Checkmark")] private Image checkmark;
    
    private float scale = 1.1f;

    public UIButtonViewController SetActiveScale(float value)
    {
        scale = value;
        return this;
    }
    
    public bool Interactable
    {
        get { return group.interactable; }
        set { group.interactable = value; }
    }
    
    public override void UpdateView()
    {
        base.UpdateView();
        
        var value = state == GenericButtonState.Active ? scale : 1f;
        var time = 0.1f * body.localScale.x / value;
        
        DOTween.Kill(body);
        body.DOScale(value, time).SetId(body);
        
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