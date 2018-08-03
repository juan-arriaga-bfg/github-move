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
		
		var particlePosition = new BoardPosition(Action.To.X, Action.To.Y, 0);
		//sequence.OnStart(() => );
		
		sequence.InsertCallback(0.05f, () => ParticleView.Show(R.MergeParticleSystem, particlePosition));
		for (int i = 0; i < points.Count; i++)
		{
			var boardElement = context.GetElementAt(points[i]);
			sequence.Insert(0, boardElement.CachedTransform.DOJump(new Vector3(to.x, to.y, boardElement.CachedTransform.position.z), 0.5f, 1, 0.25f));
			sequence.Insert(0.20f, boardElement.CachedTransform.DOScale(Vector3.zero, 0.3f));
			
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