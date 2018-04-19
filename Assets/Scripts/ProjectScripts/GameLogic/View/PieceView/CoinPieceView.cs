using DG.Tweening;
using UnityEngine;

public class CoinPieceView : PieceBoardElementView
{
    [SerializeField] private Transform body;
    [SerializeField] private Transform shadow;
    
    private readonly ViewAnimationUid AnimationId = new ViewAnimationUid();
    
    protected override void OnEnable()
    {
        base.OnEnable();
        
        DOTween.Kill(AnimationId);

        const float delay = 1f;
        var sequence = DOTween.Sequence().SetId(AnimationId).SetLoops(int.MaxValue);
        
        sequence.Insert(0, body.DOLocalMoveY(0.65f, delay).SetEase(Ease.InOutSine));
        sequence.Insert(delay, body.DOLocalMoveY(0.45f, delay).SetEase(Ease.InOutSine));
        
        sequence.Insert(0, shadow.DOScale(new Vector3(1.1f, 1.1f, 1f), delay).SetEase(Ease.InOutSine));
        sequence.Insert(delay, shadow.DOScale(new Vector3(1f, 1f, 1f), delay).SetEase(Ease.InOutSine));
    }

    private void OnDisable()
    {
        DOTween.Kill(AnimationId);
    }
}