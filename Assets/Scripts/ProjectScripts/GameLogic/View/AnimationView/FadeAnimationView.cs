using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class FadeAnimationView : AnimationView
{
    [SerializeField] private float to = 0;
    [SerializeField] private float duration = 0.2f;
    [SerializeField] private float timeoutDuration = 0.5f;
    [SerializeField] private Ease easeType = Ease.Linear;
    
    private List<SpriteRenderer> spriteRenderers = new List<SpriteRenderer>();
    public override void Play(PieceBoardElementView pieceView)
    {
        base.Play(pieceView);

        var sequence = DOTween.Sequence();
        sequence.SetId(animationUid);

        spriteRenderers = pieceView.GetCachedSpriteRenderers();
        foreach (var spriteRenderer in spriteRenderers)
        {
            sequence.Insert(0, spriteRenderer.DOFade(to, duration).SetEase(easeType));
        }   
       
        sequence.InsertCallback(timeoutDuration, () => OnComplete?.Invoke());
        
    }
}
