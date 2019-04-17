using System;
using System.Collections.Generic;

public abstract class BoardAnimation 
{
	protected ViewAnimationUid animationUid = new ViewAnimationUid();

	public Action<BoardAnimation> OnCompleteEvent { get; set; }

	public abstract void Animate(BoardRenderer context);

    public virtual void PrepareAnimation(BoardRenderer context)
    {
        
    }

	public virtual void CompleteAnimation(BoardRenderer context)
	{
		OnCompleteEvent?.Invoke(this);
		context.CompleteAnimation(this);
	}

	public virtual void StopAnimation(BoardRenderer context)
	{
	}
	
	public void ToggleView(bool state, Piece targetPiece)
	{
		var nextPiece = targetPiece;

		if (nextPiece?.ViewDefinition == null) return;

		var pathfindLockListenerComponent = nextPiece?.GetComponent<PathfindLockListenerComponent>(PathfindLockListenerComponent.ComponentGuid);
                
		var views = nextPiece.ViewDefinition.GetViews();
		for (int j = 0; j < views.Count; j++)
		{
			var view = views[j];
			
			if (pathfindLockListenerComponent != null && pathfindLockListenerComponent.IsHasPath() == false) continue;
			
			view.OnSwap(state);
		}
	}
	
	public void ToggleView(bool state, List<Piece> targetPieces)
	{
		if (targetPieces != null)
		{
			for (int i = 0; i < targetPieces.Count; i++)
			{
				var nextPiece = targetPieces[i];

				if (nextPiece?.ViewDefinition == null) continue;
                
				var views = nextPiece.ViewDefinition.GetViews();
				for (int j = 0; j < views.Count; j++)
				{
					var view = views[j];
					view.OnSwap(state);
				}
			}
		}
	}
}
