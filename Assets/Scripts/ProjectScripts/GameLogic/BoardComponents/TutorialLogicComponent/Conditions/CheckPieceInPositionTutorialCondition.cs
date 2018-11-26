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

        if (CheckAll)
        {
            var result = Positions.FindAll(CheckType);

            return CheckAbsence ? result.Count == 0 : result.Count == Positions.Count;
        }
        
        return Positions.FindIndex(position => CheckAbsence ? !CheckType(position) : CheckType(position)) != -1;
    }

    private bool CheckType(BoardPosition position)
    {
        var piece = context.Context.Context.BoardLogic.GetPieceAt(position);

        return piece != null && piece.PieceType == Target;
    }
}