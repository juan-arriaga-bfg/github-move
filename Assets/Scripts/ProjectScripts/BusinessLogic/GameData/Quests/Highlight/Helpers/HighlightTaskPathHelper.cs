using System.Collections.Generic;

public static class HighlightTaskPathHelper
{
    public static List<BoardPosition> GetAccessiblePositions(List<BoardPosition> positionsToCheck)
    {
        List<BoardPosition> ret = new List<BoardPosition>();
        
        foreach (var pos in positionsToCheck)
        {
            if (IsPositionAccessible(pos))
            {
                ret.Add(pos);
            }
        }

        return ret;
    }
    
    public static bool IsPositionAccessible(BoardPosition pos)
    {
        var board = BoardService.Current.FirstBoard;
        var boardLogic = board.BoardLogic;
        
        var piece = boardLogic.GetPieceAt(pos);
        if (piece != null && board.PathfindLocker.HasPath(piece))
        {
            return true;
        }

        return false;
    }
}