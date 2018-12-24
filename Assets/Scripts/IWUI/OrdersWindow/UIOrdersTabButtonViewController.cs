﻿using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UIOrdersTabButtonViewController : UIBaseButtonViewController
{
    [IWUIBinding] private Touchable touchable;
    [IWUIBindingNullable("#Checkmark")] private Image checkmark;
    
    private float scale = 1.1f;

    public UIButtonViewController SetActiveScale(float value)
    {
        scale = value;
        return this;
    }
    
    public bool Interactable
    {
        set { touchable.enabled = value; }
    }
    
    public override void UpdateView()
    {
        base.UpdateView();
        
        var value = state == GenericButtonState.Active ? scale : 1f;
        
        DOTween.Kill(CachedRectTransform);
        CachedRectTransform.DOScale(value, 0.1f).SetId(CachedRectTransform);
        
        if(checkmark == null) return;
        
        var alpha = state == GenericButtonState.Active ? 1f : 0f;
        
        DOTween.Kill(checkmark);
        checkmark.DOFade(alpha, 0.1f).SetId(checkmark);
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