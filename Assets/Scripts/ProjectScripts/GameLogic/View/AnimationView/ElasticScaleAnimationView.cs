using DG.Tweening;
using UnityEngine;

public class ElasticScaleAnimationView : AnimationView
{
    [SerializeField] private float timeoutDuration = 0.5f;
    [SerializeField] private Ease easeType = Ease.Linear;

    protected PieceBoardElementView pieceView = null;
    
    public override void Play(PieceBoardElementView pieceView)
    {
        base.Play(pieceView);

        var sequence = DOTween.Sequence();
        sequence.SetId(animationUid);

        sequence.Insert(0.0f, pieceView.CachedTransform.DOScale(new Vector3(1f, 0.8f, 1f), 0.1f));
        sequence.Insert(0.1f, pieceView.CachedTransform.DOScale(new Vector3(0.9f, 1.1f, 1f), 0.1f));
        sequence.Insert(0.2f, pieceView.CachedTransform.DOScale(Vector3.one, 0.1f).SetEase(easeType));
        
        sequence.InsertCallback(timeoutDuration, () => OnComplete?.Invoke());

        this.pieceView = pieceView;
    }
}