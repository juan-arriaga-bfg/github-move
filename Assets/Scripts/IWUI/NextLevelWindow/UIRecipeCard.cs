using System;
using DG.Tweening;
using UnityEngine;

public class UIRecipeCard : UISimpleScrollItem
{
    private const float ANIMATION_TIME = 0.5f;
    
    public void AddAnimation(float delay = 0)
    {
        Alpha = 0;
        CachedTransform.localScale = Vector3.zero;
        
        DOTween.Kill(this);
        
        var sequence = DOTween.Sequence().SetId(this);
        
        sequence.Insert(delay, canvas.DOFade(1, ANIMATION_TIME).SetEase(Ease.OutSine));
        sequence.Insert(delay, CachedTransform.DOScale(Vector3.one, ANIMATION_TIME).SetEase(Ease.OutElastic));
    }

    public void RemoveAnimation(float delay = 0, Action onComplete = null)
    {
        DOTween.Kill(this);

        var speed = ANIMATION_TIME * 0.5f / canvas.alpha;
        var sequence = DOTween.Sequence().SetId(this);
        
        sequence.Insert(delay, canvas.DOFade(0, speed));
        sequence.InsertCallback(delay + speed, () => onComplete?.Invoke());
    }
}