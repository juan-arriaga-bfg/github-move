using DG.Tweening;
using UnityEngine;

public class SwapPiecesAnimation : BoardAnimation 
{
	public BoardPosition PointA { get; set; }
	public BoardPosition PointB { get; set; }

	public override void Animate(BoardRenderer context)
	{
		var sequence = DOTween.Sequence().SetId(animationUid);
		
		var pieceA = context.GetElementAt(PointA);
		var pieceB = context.GetElementAt(PointB);
		
		context.SwapElements(PointA, PointB);

		Move(pieceA, sequence, context, PointA, PointB);
		Move(pieceB, sequence, context, PointB, PointA);
		
		sequence.OnComplete(() => CompleteAnimation(context));
	}

	private void Move(BoardElementView view, Sequence sequence, BoardRenderer context, BoardPosition from, BoardPosition to)
	{
		var pos = context.Context.BoardDef.GetPiecePosition(to.X, to.Y);
		pos = new Vector3(pos.x, pos.y, 0f);

		sequence.Insert(0, view.CachedTransform.DOLocalMove(pos, 0.4f).SetEase(Ease.InOutSine));
		sequence.InsertCallback(0.4f, () => context.ResetBoardElement(view, to));
	}
}