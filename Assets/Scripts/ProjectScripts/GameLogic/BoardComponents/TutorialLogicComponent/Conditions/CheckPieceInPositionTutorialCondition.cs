using System.Collections.Generic;

public class CheckPieceInPositionTutorialCondition : BaseTutorialCondition
{
    public int Target;
    public List<BoardPosition> Positions;
    public bool CheckAll;

    public override bool Check()
    {
        if (Positions == null || Positions.Count == 0) return false;
        
        foreach (var position in Positions)
        {
            var piece = context.Context.Context.BoardLogic.GetPieceAt(position);
            
            if (piece != null && piece.PieceType == Target)
            {
                if(CheckAll) continue;

                return true;
            }
            
            if(CheckAll) return false;
        }
        
        return CheckAll;
    }
}