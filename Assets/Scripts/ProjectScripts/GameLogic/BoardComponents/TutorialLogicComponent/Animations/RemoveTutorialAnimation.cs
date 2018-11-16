using System.Collections.Generic;

public class RemoveTutorialAnimation : BaseTutorialAnimation
{
    public BoardPosition Target;
    
    public override void Start()
    {
        if (isStart) return;
        
        base.Start();
        
        if(context.Context.Context.BoardLogic.IsEmpty(Target)) return;
        
        context.Context.Context.ActionExecutor.AddAction(new CollapsePieceToAction{To = Target, Positions = new List<BoardPosition>{Target}});
    }
}