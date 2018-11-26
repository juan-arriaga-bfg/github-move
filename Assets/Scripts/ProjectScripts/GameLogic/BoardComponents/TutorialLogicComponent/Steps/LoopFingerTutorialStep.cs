public class LoopFingerTutorialStep : DelayTutorialStep
{
	private TutorialMergeFinger finger;

	protected BoardPosition from;
	protected BoardPosition to;
    
	public override void PauseOn()
	{
		base.PauseOn();
		
		if(finger == null) return;
        
		Context.Context.RendererContext.DestroyElement(finger.gameObject);
		finger = null;
	}
	
	public override void Execute()
	{
		base.Execute();

		finger = Context.Context.RendererContext.CreateBoardElement<TutorialMergeFinger>((int) ViewType.TutorialMergeFinger);
		finger.Init(Context.Context.RendererContext, from, to);
	}
    
	protected override void Complete()
	{
		base.Complete();

		if (finger == null)
		{
			PauseOff();
			return;
		}
		
		Context.Context.RendererContext.DestroyElement(finger.gameObject);
	}
    
	public override bool IsExecuteable()
	{
		return finger == null && base.IsExecuteable();
	}
}