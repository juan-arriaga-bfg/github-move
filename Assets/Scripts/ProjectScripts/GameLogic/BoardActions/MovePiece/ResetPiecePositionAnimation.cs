using DG.Tweening;
using UnityEngine;

public class ResetPiecePositionAnimation : BoardAnimation 
{
	public BoardPosition At { get; set; }

    protected BoardElementView cachedPieceView;

	public override void Animate(BoardRenderer context)
	{
		var pieceFromView = context.GetElementAt(At) as PieceBoardElementView;
		
		if (pieceFromView == null)
		{
			CompleteAnimation(context);
			return;
		}

	    cachedPieceView = pieceFromView;
	    
	    context.StopMoveAnimationsAt(context, At);

		var pos = context.Context.BoardDef.GetPiecePosition(At.X, At.Y);
		pos = new Vector3(pos.x, pos.y, 0f);
		
	    pieceFromView.SyncRendererLayers(At);
		
		var sequence = DOTween.Sequence().SetId(pieceFromView.AnimationUid);
		var soundPlay = NSAudioService.Current.IsPlaying(SoundId.ObjectRelease);
		sequence.Append(pieceFromView.CachedTransform.DOLocalMove(pos, 0.4f).SetEase(Ease.InOutSine).OnUpdate(() =>
		{
			var currentPosition = pieceFromView.CachedTransform.localPosition;
			var target = context.Context.BoardDef.GetLocalPosition(pieceFromView.Piece.CachedPosition.X,
				pieceFromView.Piece.CachedPosition.Y);
				
			if (Vector2.Distance(currentPosition, target) < 0.6f && !soundPlay)
			{
				soundPlay = true;
				NSAudioService.Current.Play(SoundId.ObjectRelease);
			}
		}));
		sequence.OnComplete(() =>
		{
			
			context.ResetBoardElement(pieceFromView, At);
			CompleteAnimation(context);
		});
	}
    
    public override void StopAnimation(BoardRenderer context)
    {
        base.StopAnimation(context);

        if (cachedPieceView != null)
        {
            DOTween.Kill(cachedPieceView.AnimationUid, true);
        }
    }
}