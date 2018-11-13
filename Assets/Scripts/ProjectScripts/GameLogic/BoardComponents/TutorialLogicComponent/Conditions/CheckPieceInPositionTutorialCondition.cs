using System.Collections.Generic;

public class CheckPieceInPositionTutorialCondition : BaseTutorialCondition
{
    public int Target;
    public List<BoardPosition> Positions;
    public bool CheckAll;
    public bool CheckAbsence;

    public override bool Check()
    {
        if (Positions == null || Positions.Count == 0) return CheckAbsence;
        
        foreach (var position in Positions)
        {
            var piece = context.Context.Context.BoardLogic.GetPieceAt(position);
            
            if (piece != null && piece.PieceType == Target)
            {
                if(CheckAll) continue;

                return !CheckAbsence;
            }
            
            if(CheckAll) return CheckAbsence;
        }
        
        return CheckAbsence ? !CheckAll : CheckAll;
    }
}