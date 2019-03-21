using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Policy;
using UnityEngine;

public class AreaAccessControllerComponent:ECSEntity
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();
    public override int Guid => ComponentGuid;
    
    private HashSet<BoardPosition> basePoints;
    private HashSet<BoardPosition> availiablePositions;
    public HashSet<BoardPosition> AvailiablePositions => availiablePositions;
    
    private BoardController board;

    private static List<BoardPosition> FindConnections(BoardPosition at, List<BoardPosition> positions)
    {
        var result = new List<BoardPosition>();
        foreach (var pos in positions)
        {
            if (pos.IsNeighbor(at))
            {
                result.Add(pos);
            }
        }

        return result;
    }
    
    private static HashSet<BoardPosition> CutGroup(List<BoardPosition> positions)
    {
        var group = new HashSet<BoardPosition>();
        var uncheckedPositions = new HashSet<BoardPosition>();
        uncheckedPositions.Add(positions.First());
        
        while (uncheckedPositions.Count > 0)
        {
            var current = uncheckedPositions.First();
            uncheckedPositions.Remove(current);
            group.Add(current);
            positions.Remove(current);
            foreach (var connection in FindConnections(current, positions))
            {
                if (group.Contains(connection) == false && uncheckedPositions.Contains(connection) == false)
                {
                    uncheckedPositions.Add(connection);   
                }
            }
        }

        return group;
    }
    
    private HashSet<BoardPosition> DetectBasePoints(BoardController board)
    {
#if DEBUG
        var sw = new Stopwatch();
        sw.Start();
#endif
        var empty = board.BoardLogic.FieldFinder.FindWhere((pos, logic) =>
        {
            var piece = logic.GetPieceAt(pos);
            if (piece != null && piece.Draggable?.IsDraggable(piece.CachedPosition) != true)
            {
                return false;
            }
            return true;
        });
        
        //Group zones;
        var currentMaxGroup = new HashSet<BoardPosition>();
        var currentCheckGroup = CutGroup(empty);
        while (currentCheckGroup.Count > 0)
        {
            if (currentCheckGroup.Count > currentMaxGroup.Count)
            {
                currentMaxGroup = currentCheckGroup;
            }
                
            currentCheckGroup = empty.Count > 0 ? CutGroup(empty) : new HashSet<BoardPosition>();
        }
#if DEBUG
        sw.Stop();
        IW.Logger.Log($"[AreaAccessControllerComponent] => Detect base points calculated: {sw.ElapsedMilliseconds}ms");
#endif
        return currentMaxGroup;
    }
    
    public override void OnRegisterEntity(ECSEntity entity)
    {
        board = entity as BoardController;
    }

    public void FullRecalculate()
    {
        if (basePoints == null || basePoints.Count == 0)
        {
            basePoints = DetectBasePoints(board);
        }
        
        AvailiablePositions?.Clear();
        availiablePositions = new HashSet<BoardPosition>(basePoints);
        foreach (var basePos in basePoints)
        {
            var resultArea = board.BoardLogic.EmptyCellsFinder.FindEmptyAreaByPoint(basePos);
            foreach (var pos in resultArea)
            {
                AvailiablePositions?.Add(pos);
            }
        }
        
    }
    
    public void LocalRecalculate(BoardPosition changedPosition)
    {
        var fields = new List<BoardPosition>();
        var emptyFinder = board.BoardLogic.EmptyCellsFinder;
        fields = emptyFinder.FindAreaByPoint(changedPosition, 
                                             (pos, controller) => AvailiablePositions.Contains(pos) == false 
                                                                  && controller.BoardLogic.GetPieceAt(pos) == null);

        foreach (var field in fields)
        {
            AvailiablePositions.Add(field);
        }
        
    }
}
