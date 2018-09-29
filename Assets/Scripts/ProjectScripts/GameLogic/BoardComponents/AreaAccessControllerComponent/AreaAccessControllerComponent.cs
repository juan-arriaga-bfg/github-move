using System.Collections.Generic;
using System.Security.Policy;

public class AreaAccessControllerComponent:ECSEntity
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();
    public override int Guid => ComponentGuid;

    private BoardController board;

    private HashSet<BoardPosition> InitAvailiable(BoardController board)
    {
        var logic = board.BoardLogic;
        var resultSet = new HashSet<BoardPosition>();
        resultSet.Add(new BoardPosition(18,18));
        return resultSet;
    }
    
    public override void OnRegisterEntity(ECSEntity entity)
    {
        board = entity as BoardController;
        basePoints = InitAvailiable(board);
        availiablePoints = new HashSet<BoardPosition>(basePoints);
    }

    private HashSet<BoardPosition> basePoints;
    private HashSet<BoardPosition> availiablePoints;
    public HashSet<BoardPosition> AvailiablePositions
    {
        get { return availiablePoints; }
    }

    public void FullRecalculate()
    {
        availiablePoints = new HashSet<BoardPosition>(basePoints);
        var closedPositions = new HashSet<BoardPosition>();
        var border = GetBorderPoints(availiablePoints, closedPositions);
        while (border.Count != 0)
        {
            foreach (var pos in border)
            {
                if (board.BoardLogic.IsLockedCell(pos) == false)
                    availiablePoints.Add(pos);
                else
                    closedPositions.Add(pos);
            }

            border = GetBorderPoints(availiablePoints, closedPositions);
        }
    }

    private HashSet<BoardPosition> GetBorderPoints(HashSet<BoardPosition> currentPositions, HashSet<BoardPosition> closedPositions)
    {
        var border = new HashSet<BoardPosition>();
        foreach (var pos in currentPositions)
        {
            var neighbors = pos.Neighbors();
            foreach (var neigh in neighbors)
            {
                if (board.BoardLogic.IsPointValid(neigh) &&
                    currentPositions.Contains(neigh) == false && closedPositions.Contains(neigh) == false)
                    border.Add(neigh);
            }
        }
        return border;
    }
    
    public void LocalRecalculate(List<BoardPosition> changedPositions)
    {
        //TODO create optimized recalc
        FullRecalculate();
    }
}
