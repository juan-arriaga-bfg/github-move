using DG.Tweening;
using UnityEngine;

public class SpawnPiceAndJumpAnimation : BoardAnimation 
{
    public Piece Piece;
    
    public BoardPosition From;
    public BoardPosition To;
    
    public override void Animate(BoardRenderer context)
    {
        var to = context.Context.BoardDef.GetPiecePosition(To.X, To.Y);
        var element = context.CreatePieceAt(Piece, To);
            
        element.CachedTransform.localScale = Vector3.zero;
        element.CachedTransform.localPosition = context.Context.BoardDef.GetPiecePosition(From.X, From.Y);
        
        var sequence = DOTween.Sequence().SetId(animationUid);
        
        sequence.Insert(0, element.CachedTransform.DOJump(new Vector3(to.x, to.y, element.CachedTransform.position.z), 1, 1, 0.4f).SetEase(Ease.InOutSine));
        sequence.Insert(0, element.CachedTransform.DOScale(Vector3.one * 1.3f, 0.2f));
        sequence.Insert(0.2f, element.CachedTransform.DOScale(Vector3.one, 0.2f));
            
        sequence.Insert(0.4f, element.CachedTransform.DOScale(new Vector3(1f, 0.8f, 1f), 0.1f));
        sequence.Insert(0.5f, element.CachedTransform.DOScale(new Vector3(0.9f, 1.1f, 1f), 0.1f));
        sequence.Insert(0.6f, element.CachedTransform.DOScale(Vector3.one, 0.1f).SetEase(Ease.OutBack));
        
        sequence.OnComplete(() =>
        {
            CompleteAnimation(context);
        });
    }
}