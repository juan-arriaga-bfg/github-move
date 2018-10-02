using System.Collections.Generic;
using System.Security.Policy;
using UnityEngine;

public class AreaAccessControllerComponent:ECSEntity
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();
    public override int Guid => ComponentGuid;

    private BoardController board;

    private HashSet<BoardPosition> InitAvailiable(BoardController board)
    {
        var logic = board.BoardLogic;
        var resultSet = new HashSet<BoardPosition>();
        resultSet.Add(new BoardPosition(23, 10, 1));
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
    public HashSet<BoardPosition> AvailiablePositions => availiablePoints;

    public void FullRecalculate()
    {
        AvailiablePositions.Clear();
        foreach (var basePos in basePoints)
        {
            var resultArea = board.BoardLogic.EmptyCellsFinder.FindEmptyAreaByPoint(basePos);
            foreach (var pos in resultArea)
            {
                AvailiablePositions.Add(pos);
            }
        }
        
    }

    
    
    public void LocalRecalculate(BoardPosition changedPosition)
    {
        var fields = new List<BoardPosition>();
        fields = board.BoardLogic.EmptyCellsFinder.FindEmptyAreaByPoint(changedPosition);

        foreach (var field in fields)
        {
            AvailiablePositions.Add(field);
        }
        
    }
}
