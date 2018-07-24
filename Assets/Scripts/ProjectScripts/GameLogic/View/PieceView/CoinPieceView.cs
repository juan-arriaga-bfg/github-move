using DG.Tweening;
using UnityEngine;

public class CoinPieceView : PieceBoardElementView
{
    [SerializeField] private Transform body;
    
    private readonly ViewAnimationUid AnimationId = new ViewAnimationUid();
    
    protected override void OnEnable()
    {
        base.OnEnable();
        
        DOTween.Kill(AnimationId);

        const float delay = 1f;
        var sequence = DOTween.Sequence().SetId(AnimationId).SetLoops(int.MaxValue);
        
        sequence.Insert(0, body.DOLocalMoveY(0.65f, delay).SetEase(Ease.InOutSine));
        sequence.Insert(delay, body.DOLocalMoveY(0.45f, delay).SetEase(Ease.InOutSine));
    }

    private void OnDisable()
    {
        DOTween.Kill(AnimationId);
    }
}