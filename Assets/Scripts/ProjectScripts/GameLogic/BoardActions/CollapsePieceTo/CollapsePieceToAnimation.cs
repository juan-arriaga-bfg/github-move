using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class CollapsePieceToAnimation : BoardAnimation
{
	public BoardPosition To;
	public List<BoardPosition> Positions;
    public float Delay = 0f;
    
    public Func<int, string> AnimationResourceSearch;
	
	public override void Animate(BoardRenderer context)
	{
		var points = Positions;
		var boardElements = new List<BoardElementView>();
		
		var to = context.Context.BoardDef.GetPiecePosition(To.X, To.Y);
		
		var sequence = DOTween.Sequence().SetId(animationUid);
		var completeCount = 0;

		void Complete()
		{
			completeCount++;
			
			if (completeCount != points.Count) return;

			foreach (var element in boardElements)
			{
				context.DestroyElement(element);
			}
			
			CompleteAnimation(context);
		}
		
		foreach (var point in points)
		{
			var boardElement = context.RemoveElementAt(point, false);
			
			if (boardElement == null)
			{
				sequence.InsertCallback(0.3f, Complete);
				continue;
			}
			
			boardElements.Add(boardElement);
			
			if (To.IsValid) boardElement.SyncRendererLayers(context.Context.BoardDef.MaxPoit);
			
			sequence.Insert(0 + Delay, boardElement.CachedTransform.DOMove(new Vector3(to.x, to.y, boardElement.CachedTransform.position.z), 0.4f).SetEase(Ease.OutBack));
			
			if (boardElement is PieceBoardElementView pieceBoardElement)
			{
				var animationResource = AnimationResourceSearch?.Invoke(pieceBoardElement.Piece.PieceType);
				
				if (string.IsNullOrEmpty(animationResource) == false)
				{
					sequence.InsertCallback(0.2f, () =>
					{
						var animView = context.CreateBoardElementAt<AnimationView>(animationResource, point);
						animView.Play(pieceBoardElement);
						animView.OnComplete += Complete;
					});					
					
					continue;
				}
			}
			
			sequence.Insert(0.2f + Delay, boardElement.CachedTransform.DOScale(Vector3.zero, 0.3f).SetEase(Ease.OutBack));
			sequence.InsertCallback(0.3f, Complete);
		}
	}
}