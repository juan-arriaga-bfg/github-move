using System.Collections.Generic;

public class RemoveTutorialAnimation : BaseTutorialAnimation
{
    public int PieceId = PieceType.None.Id;
    public BoardPosition Target;
    
    public override void Start()
    {
        if (isStart) return;
        
        base.Start();
        
        if(Find()) return;

        Hard(Target);
    }

    private bool Find()
    {
        if(PieceId == PieceType.None.Id) return false;

        var positions = context.Context.Context.BoardLogic.PositionsCache.GetPiecePositionsByType(PieceId);

        foreach (var position in positions)
        {
            Hard(position);
        }
        
        return true;
    }
    
    private void Hard(BoardPosition target)
    {
        var board = context.Context.Context;
        
        if(board.BoardLogic.IsEmpty(target)) return;
        
        board.ActionExecutor.AddAction(new CollapsePieceToAction{To = target, Positions = new List<BoardPosition>{target}});
    }
}