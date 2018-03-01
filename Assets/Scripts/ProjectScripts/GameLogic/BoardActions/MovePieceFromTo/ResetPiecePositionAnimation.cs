using DG.Tweening;
using UnityEngine;

public class ResetPiecePositionAnimation : BoardAnimation 
{
	public BoardPosition At { get; set; }

	public override void Animate(BoardRenderer context)
	{
		var pieceFromView = context.GetElementAt(At);

		var pos = context.Context.BoardDef.GetPiecePosition(At.X, At.Y);
		pos = new Vector3(pos.x, pos.y, 0f);
		
		var sequence = DOTween.Sequence().SetId(pieceFromView.AnimationUid);
		sequence.Append(pieceFromView.CachedTransform.DOLocalMove(pos, 0.4f).SetEase(Ease.InOutSine));
		sequence.OnComplete(() =>
		{
			context.ResetBoardElement(pieceFromView, At);
			
			CompleteAnimation(context);
		});

	}
}