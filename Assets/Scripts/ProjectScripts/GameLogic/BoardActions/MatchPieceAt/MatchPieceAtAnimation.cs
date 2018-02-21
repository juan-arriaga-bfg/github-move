using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class MatchPieceAtAnimation : BoardAnimation 
{
	public MatchPieceAtAction Action { get; set; }
    

	public override void Animate(BoardRenderer context)
	{
		var points = Action.MatchField;
		var to = context.Context.BoardDef.GetPiecePosition(Action.At.X, Action.At.Y);
		
		var sequence = DOTween.Sequence().SetId(animationUid);
		
		for (int i = 0; i < points.Count; i++)
		{
			var boardElement = context.GetElementAt(points[i]);
			
			sequence.Insert(0, boardElement.CachedTransform.DOMove(new Vector3(to.x, to.y, boardElement.CachedTransform.position.z), 0.4f).SetEase(Ease.OutBack));
			sequence.Insert(0.2f, boardElement.CachedTransform.DOScale(Vector3.zero, 0.3f).SetEase(Ease.OutBack));
		}
		
		sequence.OnComplete(() =>
		{
			for (int i = 0; i < points.Count; i++)
			{
				context.RemoveElementAt(points[i]);
			}
			
			CompleteAnimation(context);
		});
	}
}