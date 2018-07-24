using System;

public abstract class BoardAnimation 
{
	protected ViewAnimationUid animationUid = new ViewAnimationUid();

	public Action<BoardAnimation> OnCompleteEvent { get; set; }

	public abstract void Animate(BoardRenderer context);

	public virtual void CompleteAnimation(BoardRenderer context)
	{
		if (OnCompleteEvent != null)
		{
			OnCompleteEvent(this);
		}

		context.CompleteAnimation(this);
	}

	public virtual void StopAnimation(BoardRenderer context)
	{
		
	}
}
