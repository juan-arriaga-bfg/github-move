using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class MovePiecesFromToAnimation : BoardAnimation 
{
	public List<BoardPosition> From { get; set; }
	
	public List<BoardPosition> To { get; set; }

	public override void Animate(BoardRenderer context)
	{
		if (From == null || To == null || From.Count != To.Count)
		{
#if UNITY_EDITOR
			Debug.LogError("Error: list From or To == null, or From.Count != To.Count");
#endif
			return;
		}

		var sequence = DOTween.Sequence().SetId(animationUid);
		
		for (int i = 0; i < From.Count; i++)
		{
			var from = From[i];
			var to = To[i];
			
			var pieceFromView = context.GetElementAt(from);

			context.MoveElement(from, to);
        
			var pos = context.Context.BoardDef.GetPiecePosition(to.X, to.Y);
			pos = new Vector3(pos.x, pos.y, 0f);

			sequence.Insert(0, pieceFromView.CachedTransform.DOLocalMove(pos, 0.4f).SetEase(Ease.InOutSine));
			sequence.InsertCallback(0.4f, () => context.ResetBoardElement(pieceFromView, to));
		}
		
		sequence.OnComplete(() => CompleteAnimation(context));
	}
}