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
		
		foreach (var point in points)
		{
			var boardElement = context.GetElementAt(point);

			boardElement.SyncRendererLayers(context.Context.BoardDef.MaxPoit);

		    if (Action.To.IsValid)
		    {
		        sequence.Insert(0 + Delay, boardElement.CachedTransform.DOMove(new Vector3(to.x, to.y, boardElement.CachedTransform.position.z), 0.4f).SetEase(Ease.OutBack));
		    }

		    sequence.Insert(0.2f + Delay, boardElement.CachedTransform.DOScale(Vector3.zero, 0.3f).SetEase(Ease.OutBack));
		}
		
		sequence.OnComplete(() =>
		{
			foreach (var point in points)
			{
				context.RemoveElementAt(point);
			}

			CompleteAnimation(context);
		});
	}
}