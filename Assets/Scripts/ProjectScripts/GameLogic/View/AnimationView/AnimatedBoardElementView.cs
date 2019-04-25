public class AnimatedBoardElementView : BoardElementView
{
    public virtual void PlayShow()
    {
    }

    public virtual void PlayIdle()
    {
    }

    public virtual void PlayHide()
    {
        Context.DestroyElement(this);
    }
}