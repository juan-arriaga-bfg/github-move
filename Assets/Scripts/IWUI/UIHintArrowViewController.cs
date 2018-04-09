using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class UIHintArrowViewController : IWUIWindowViewController 
{
    [SerializeField] private CanvasGroup viewCanvasGroup;
    
    public void FadeInOut(bool state)
    {
        if (state)
        {
            viewCanvasGroup.alpha = 0f;
        }
        DOTween.Kill(viewCanvasGroup);
        var sequence = DOTween.Sequence().SetId(viewCanvasGroup);
        sequence.Insert(0, viewCanvasGroup.DOFade(state ? 1f : 0f, 0.35f));
    }
}
