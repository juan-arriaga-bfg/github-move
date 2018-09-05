using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using DG.Tweening.Plugins.Core.PathCore;
using Org.BouncyCastle.Utilities.Collections;
using UnityEngine;

public class PathfinderComponent:ECSEntity
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();
    public override int Guid => ComponentGuid;

    private BoardController board;
    
    public override void OnRegisterEntity(ECSEntity entity)
    {
        board = entity as BoardController;
    }

    protected Predicate<BoardPosition> GetCondition(Piece piece)
    {
        var boardCondition = piece?.BoardCondition;
        if (boardCondition == null)
            return (_) => true;
        return boardCondition.Check;
    }
    
    
    //A* pathfinding algorithm
    public virtual bool HasPath(BoardPosition from, HashSet<BoardPosition> to,
        Piece piece = null)
    {

        if (to.Contains(from))
            return true;
        
        to = new HashSet<BoardPosition>(to.Where(elem => from.Z == elem.Z));
        if (to.Count == 0)
            return false;
        
        //Init locals
        var checkedPositions = new HashSet<BoardPosition>();
        var uncheckedPositions = new HashSet<BoardPosition>();

        var costMap = new Dictionary<BoardPosition, int>();
        var predictionCosts = new Dictionary<BoardPosition, int>();

        Predicate<BoardPosition> fieldCondition = GetCondition(piece);
        
        //Init start node data
        uncheckedPositions.Add(from);

        costMap[from] = 0;
        predictionCosts[from] = Heuristic(from, to.First());
        
        //Begin pathfinding
        while (uncheckedPositions.Count > 0)
        {
            var current = FindPosWithMinimalCost(predictionCosts, uncheckedPositions);
            if (to.Contains(current))
                return true;
            
            uncheckedPositions.Remove(current);
            checkedPositions.Add(current);

            var availiablePositions = AvailiablePositions(current, checkedPositions, fieldCondition);
            
            //Init neighbour positions data
            for (int i = 0; i < availiablePositions.Count; i++)
            {
                var currentNeghbour = availiablePositions[i];
                
                int distance = 1;
                var tempCost = costMap[current] + distance;

                if (!uncheckedPositions.Contains(currentNeghbour) || tempCost < costMap[currentNeghbour])
                {
                    costMap[currentNeghbour] = tempCost;
                    predictionCosts[currentNeghbour] = tempCost + Heuristic(currentNeghbour, to.First());
                }
                
                if(!uncheckedPositions.Contains(currentNeghbour))
                    uncheckedPositions.Add(currentNeghbour);
            }
        }
        Debug.Log($"{from} > finish false");
        return false;
    }
    
    public virtual bool HasPath(BoardPosition from, BoardPosition to, Piece piece = null)
    {
        return HasPath(from, new HashSet<BoardPosition> {to}, piece);
    }
    
    protected BoardPosition FindPosWithMinimalCost(Dictionary<BoardPosition, int> costs,
        HashSet<BoardPosition> uncheckedPositions)
    {
        var currentPos = uncheckedPositions.First();
        var minimalCost = costs[currentPos];

        foreach (var position in uncheckedPositions)
        {
            if (costs[position] < minimalCost)
            {
                minimalCost = costs[position];
                currentPos = position;
            }  
        }

        return currentPos;
    }

    protected virtual int Heuristic(BoardPosition from, BoardPosition to)
    {
        return Math.Abs(from.X - to.X) + Math.Abs(from.Y - to.Y);
    }
    
    protected List<BoardPosition> AvailiablePositions(BoardPosition position, HashSet<BoardPosition> checkedPositions, Predicate<BoardPosition> predicate)
    {
        var uncheckedNeigbours = new List<BoardPosition>
        {
            position.Right,
            position.Up,
            position.Left,
            position.Down
        };
        
        var checkedNeigbours = new List<BoardPosition>();

        for (var i = 0; i < uncheckedNeigbours.Count; i++)
        {
            var currentNeighbour = uncheckedNeigbours[i];
            if(!checkedPositions.Contains(currentNeighbour) && predicate.Invoke(currentNeighbour))
                checkedNeigbours.Add(currentNeighbour); 
        }
        
        return checkedNeigbours;
    }
}

public class PathfindLockerComponent : ECSEntity
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();
    public override int Guid => ComponentGuid;

    private BoardController context;

    private PathfinderComponent pathfinder;
    public PathfinderComponent Pathfinder => pathfinder ??
                                             (pathfinder = context?.Pathfinder);

    private List<Piece> lastPathFoundedPieces;
    
    private List<int> ids;
    
    public override void OnRegisterEntity(ECSEntity entity)
    {
        base.OnRegisterEntity(entity);
        context = entity as BoardController;
        ids = PieceType.GetIdsByFilter(PieceTypeFilter.Character);
    }
    
    private Dictionary<Piece, bool> cachedPathes = new Dictionary<Piece, bool>();
    
    public virtual bool HasPath(Piece piece)
    {
        return cachedPathes.ContainsKey(piece) && cachedPathes[piece];
    }

    public virtual List<Piece> RecalcCache(BoardPosition targetPosition)
    {
        List<Piece> piecesWithChangedPath = new List<Piece>();

        foreach (var piece in GetTargetPieces())
        {
            var canPath = Pathfinder.HasPath(piece.CachedPosition, targetPosition, piece);
            if (cachedPathes.ContainsKey(piece) == false || cachedPathes[piece] != canPath)
                piecesWithChangedPath.Add(piece);

            cachedPathes[piece] = canPath;
        }

        return piecesWithChangedPath;
    }

    private List<Piece> GetTargetPieces()
    {
        List<Piece> target = new List<Piece>();
        foreach (var id in ids)
        {
            var positions = context.BoardLogic.PositionsCache.GetPiecePositionsByType(id);
            foreach (var position in positions)
            {
                target.Add(context.BoardLogic.GetPieceAt(position));
            }
        }

        return target;
    }

    public void Step()
    {
        if (lastPathFoundedPieces == null)
            lastPathFoundedPieces = GetTargetPieces();

        foreach (var piece in lastPathFoundedPieces)
        {
            var lockCheck = piece.Draggable?.Locker.IsLocked;
            Debug.Log($"piece on {piece.CachedPosition}, lockCheck {lockCheck}");
        }

        var position = context.BoardLogic.PositionsCache.GetRandomPositions(PieceType.Char1.Id, 1)[0];
        Debug.Log($"TargetPosition {position}");
        lastPathFoundedPieces = RecalcCache(position);

        foreach (var piece in lastPathFoundedPieces) 
        {
            var canPath = HasPath(piece);
            if(canPath)
                piece.Draggable.Locker.Unlock(this, true);
            else
                piece.Draggable.Locker.Lock(this);
        }
    }
}