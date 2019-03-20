using UnityEngine;
using DG.Tweening;

public class MovePieceFromToAnimation : BoardAnimation 
{
    public BoardPosition From { get; set; }
	
    public BoardPosition To { get; set; }
    
    public override void Animate(BoardRenderer context)
    {
        var pieceFromView = context.GetElementAt(From);

        context.MoveElement(From, To);
        
        var pos = context.Context.BoardDef.GetPiecePosition(To.X, To.Y);
        pos = new Vector3(pos.x, pos.y, 0f);
        
        // pieceFromView.SyncRendererLayers(context.Context.BoardDef.MaxPoit);
        
        pieceFromView.SyncRendererLayers(To);        

        var sequence = DOTween.Sequence().SetId(animationUid);
        sequence.Append(pieceFromView.CachedTransform.DOLocalMove(pos, 0.4f).SetEase(Ease.InOutSine));
        sequence.OnComplete(() =>
        {
            context.ResetBoardElement(pieceFromView, To);
            CompleteAnimation(context);
        });
    }

    public override void StopAnimation(BoardRenderer context)
    {
        base.StopAnimation(context);

        DOTween.Kill(animationUid, true);
    }
}