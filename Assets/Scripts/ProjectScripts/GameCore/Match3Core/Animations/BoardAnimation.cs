using System;

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
}
