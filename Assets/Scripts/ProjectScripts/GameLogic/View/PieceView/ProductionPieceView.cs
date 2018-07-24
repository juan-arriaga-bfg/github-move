using DG.Tweening;
using UnityEngine;

public class ProductionPieceView : PieceBoardElementView
{
    [SerializeField] private SpriteRenderer outline;
    
    private readonly ViewAnimationUid outlineAnimationId = new ViewAnimationUid();
    
    public override void Init(BoardRenderer context, Piece piece)
    {
        base.Init(context, piece);

        ChangeOutline(false);
    }

    public void ChangeOutline(bool isStart)
    {
        DOTween.Kill(outlineAnimationId);

        outline.DOFade(isStart ? 1 : 0, 0);
        
        if(isStart == false) return;
        
        var sequence = DOTween.Sequence().SetId(outlineAnimationId).SetLoops(int.MaxValue);
        sequence.Append(outline.DOFade(0, 0.4f));
        sequence.Append(outline.DOFade(1, 0.3f));
    }
}