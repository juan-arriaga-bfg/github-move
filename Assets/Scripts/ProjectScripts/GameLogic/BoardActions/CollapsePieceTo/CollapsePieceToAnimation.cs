using DG.Tweening;
using UnityEngine;

public class CollapsePieceToAnimation : BoardAnimation
{
	public CollapsePieceToAction Action { get; set; }
	
	public override void Animate(BoardRenderer context)
	{
		var points = Action.Positions;
		var to = context.Context.BoardDef.GetPiecePosition(Action.To.X, Action.To.Y);
		
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

public class MatchPieceToAnimation : BoardAnimation
{
	public CollapsePieceToAction Action { get; set; }
	
	public override void Animate(BoardRenderer context)
	{
		var points = Action.Positions;
		var to = context.Context.BoardDef.GetPiecePosition(Action.To.X, Action.To.Y);
		
		var sequence = DOTween.Sequence().SetId(animationUid);
		var elementOffset = 0.05f;
		//sequence.timeScale = 0.5f;
		var particlePosition = new BoardPosition(Action.To.X, Action.To.Y, 2);
		sequence.timeScale = 1.2f;
		sequence.InsertCallback(0.0f, () => ParticleView.Show(R.SmolderingParticles, particlePosition));
		sequence.InsertCallback(0.0f, () => ParticleView.Show(R.MergeParticleSystem, particlePosition));
		for (int i = 0; i < points.Count; i++)
		{
			var boardElement = context.GetElementAt(points[i]);
			if (points[i].Equals(Action.To))
			{
				sequence.Insert(0.15f, boardElement.CachedTransform.DOScale(Vector3.one * 1.2f, 0.20f));
				//sequence.Insert(0.35f, boardElement.CachedTransform.DOScale(Vector3.zero, 0.1f));
				sequence.Insert(0.35f + points.Count * elementOffset, boardElement.CachedTransform.DOScale(Vector3.zero, 0.1f));
				boardElement.SyncRendererLayers(new BoardPosition(Action.To.X, Action.To.Y, 4));
				continue;
			}
			
			sequence.Insert(0 + elementOffset*i, boardElement.CachedTransform.DOMove(new Vector3(to.x, to.y, boardElement.CachedTransform.position.z), 0.25f));
			sequence.Insert(0.20f + elementOffset*i, boardElement.CachedTransform.DOScale(Vector3.zero, 0.20f));
			
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