using DG.Tweening;
using UnityEngine;

public class ElasticScaleAnimationView : AnimationView
{
    [SerializeField] private float timeoutDuration = 0.5f;
    
    public override void Play(PieceBoardElementView pieceView)
    {
        base.Play(pieceView);

        var sequence = DOTween.Sequence();
        sequence.SetId(animationUid);

        pieceView.CachedTransform.localScale = Vector3.zero;
        sequence.Insert(0.1f, pieceView.CachedTransform.DOScale(Vector3.one * 1.2f, 0.4f));
        sequence.Insert(0.6f, pieceView.CachedTransform.DOScale(Vector3.one, 0.3f));
        
        sequence.InsertCallback(timeoutDuration, CompleteAnimation);
    }
}