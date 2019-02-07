using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class ScaleAndFadeAnimationView : AnimationView
{
    [SerializeField] private float toFade = 0;
    [SerializeField] private Vector3 toScale = new Vector3(1, 1, 1);
    [SerializeField] private float scaleDuration = 0.2f;
    [SerializeField] private float fadeDuration = 0.2f;
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
            sequence.Insert(0, spriteRenderer.DOFade(toFade, fadeDuration).SetEase(easeType));
        }

        sequence.Insert(0, pieceView.transform.DOScale(toScale, scaleDuration).SetEase(easeType));
       
        sequence.InsertCallback(timeoutDuration, () => OnComplete?.Invoke());
        
    }
}
