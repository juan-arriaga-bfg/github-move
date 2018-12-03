using System.Collections.Generic;

public static class HighlightTaskPathHelper
{
    public static List<BoardPosition> GetAccessiblePositions(List<BoardPosition> positionsToCheck)
    {
        var board      = BoardService.Current.FirstBoard;
        var boardLogic = board.BoardLogic;
        
        List<BoardPosition> ret = new List<BoardPosition>();
        
        foreach (var pos in positionsToCheck)
        {
            var piece = boardLogic.GetPieceAt(pos);
            if (piece != null && board.Pathfinder.CanPathToCastle(piece))
            {
                ret.Add(pos);
            }
        }

        return ret;
    }
}