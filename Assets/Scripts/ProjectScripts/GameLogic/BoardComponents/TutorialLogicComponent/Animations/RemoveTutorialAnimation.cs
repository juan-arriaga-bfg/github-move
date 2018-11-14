using System.Collections.Generic;

public class RemoveTutorialAnimation : BaseTutorialAnimation
{
    public BoardPosition Target;
    
    public override void Start()
    {
        if (isStart) return;
        
        base.Start();
        
        context.Context.Context.ActionExecutor.AddAction(new CollapsePieceToAction{To = Target, Positions = new List<BoardPosition>{Target}});
    }
}