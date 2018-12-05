using System;
using DG.Tweening;
using UnityEngine;

public class CollapsePieceToAnimation : BoardAnimation
{
	public CollapsePieceToAction Action { get; set; }

    public float Delay = 0f;
	
	public override void Animate(BoardRenderer context)
	{
		var points = Action.Positions;
		var to = context.Context.BoardDef.GetPiecePosition(Action.To.X, Action.To.Y);
		
		var sequence = DOTween.Sequence().SetId(animationUid);

		var completeCount = 0;
		Action complete = () =>
		{
			completeCount++;
			if (completeCount == points.Count)
			{
				foreach (var boardPosition in points)
				{
					context.RemoveElementAt(boardPosition);	
				}

				CompleteAnimation(context);
			}
		};
		
		
		foreach (var point in points)
		{
			var boardElement = context.GetElementAt(point);

			if (Action.To.IsValid)
				boardElement.SyncRendererLayers(context.Context.BoardDef.MaxPoit);
			
			sequence.Insert(0 + Delay, boardElement.CachedTransform.DOMove(new Vector3(to.x, to.y, boardElement.CachedTransform.position.z), 0.4f).SetEase(Ease.OutBack));
			if (boardElement is PieceBoardElementView)
			{
				var pieceBoardElement = (PieceBoardElementView)boardElement;
				
				var animationResource = Action.AnimationResourceSearch?.Invoke(pieceBoardElement.Piece.PieceType);
				if (string.IsNullOrEmpty(animationResource) == false)
				{
					
					sequence.InsertCallback(0.2f, () =>
					{
						var animView = context.CreateBoardElementAt<AnimationView>(animationResource, point);
						animView.Play(pieceBoardElement);
						animView.OnComplete += complete;
					});					
					
					continue;
				}
			
			}
			sequence.Insert(0.2f + Delay, boardElement.CachedTransform.DOScale(Vector3.zero, 0.3f).SetEase(Ease.OutBack));
			sequence.InsertCallback(0.3f, () => complete());
		}
	}
}