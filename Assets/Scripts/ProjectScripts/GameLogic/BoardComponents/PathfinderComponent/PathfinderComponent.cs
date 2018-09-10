using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    public virtual bool HasPath(BoardPosition from, HashSet<BoardPosition> to, out List<BoardPosition> blockagePositions,
        Piece piece = null)
    {
        blockagePositions = new List<BoardPosition>();
        
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

            var availiablePositions = AvailiablePositions(current, checkedPositions, fieldCondition, ref blockagePositions);
            
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
        
        return false;
    }
    
    public virtual bool HasPath(BoardPosition from, BoardPosition to, out List<BoardPosition> blockagePositions, Piece piece = null)
    {
        return HasPath(from, new HashSet<BoardPosition> {to}, out blockagePositions, piece);
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
    
    protected List<BoardPosition> AvailiablePositions(BoardPosition position, HashSet<BoardPosition> checkedPositions, Predicate<BoardPosition> predicate, ref List<BoardPosition> unavailiable)
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
            else if(!unavailiable.Contains(currentNeighbour) && board.BoardLogic.GetPieceAt(currentNeighbour) != null)
                unavailiable.Add(currentNeighbour);
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

    private Dictionary<Piece, List<BoardPosition>> blockPathPieces = new Dictionary<Piece, List<BoardPosition>>();
    private List<Piece> freePieces = new List<Piece>();
    private BoardPosition lastCheckedPosition = BoardPosition.Default();
    
    public override void OnRegisterEntity(ECSEntity entity)
    {
        base.OnRegisterEntity(entity);
        context = entity as BoardController;
    }
    
    public virtual bool HasPath(Piece piece)
    {
        return !blockPathPieces.ContainsKey(piece) && freePieces.Contains(piece); 
    }

    public virtual void RecalcCacheOnPieceAdded(BoardPosition target, BoardPosition changedPosition, Piece piece, bool autoLock)
    {
        lastCheckedPosition = target;
        
        RecalcFree(target, changedPosition);

        
        if (autoLock)
        {
            var addedPiece = piece;
            RecalcFor(addedPiece, target);
        }   
    }

    private bool RecalcFor(Piece piece, BoardPosition target)
    {
        List<BoardPosition> pieceBlockers;
        var canPath = Pathfinder.HasPath(piece.CachedPosition, target, out pieceBlockers, piece); 
        if (canPath && !freePieces.Contains(piece))
        {
            blockPathPieces.Remove(piece);
            freePieces.Add(piece);
            piece.Draggable?.Locker?.Unlock(this, true);
        }
        else if (canPath == false)
        {
            freePieces.Remove(piece);
            if(blockPathPieces.ContainsKey(piece) == false)
                piece.Draggable?.Locker?.Lock(this);
            blockPathPieces[piece] = pieceBlockers;
        }

#if DEBUG
        ShowStatus();
#endif
        
        return canPath;
    }

#if DEBUG
    private void ShowStatus()
    {
        StringBuilder builder = new StringBuilder();

        builder.Append("free: ");
        foreach (var free in freePieces)
        {
            builder.Append($"{free.CachedPosition}, ");
        }

        builder.Append("\nblocked: ");
        foreach (var block in blockPathPieces.Keys)
        {
            builder.Append($"\n\t{block.CachedPosition}: {string.Join(",", blockPathPieces[block])}");
        }
        
        Debug.LogError($"CacheStatus\n\n{builder}\n\n");

    }
#endif
    
    public virtual void RecalcCacheOnPieceRemoved(BoardPosition target , BoardPosition changedPosition, Piece removedPiece)
    {
        if (changedPosition.Equals(lastCheckedPosition))
        {
            RecalcAll(target);
            return;
        }

        if (freePieces.Contains(removedPiece))
            freePieces.Remove(removedPiece);
        if (blockPathPieces.ContainsKey(removedPiece))
            freePieces.Remove(removedPiece);

        RecalcBlocked(target, changedPosition);
    }

    public virtual void RecalcCacheOnPieceMoved(BoardPosition target, BoardPosition fromPosition, BoardPosition to, Piece piece,
        bool autoLock)
    {
        if (fromPosition.Equals(lastCheckedPosition))
        {
            RecalcAll(target);
            return;
        }

        var pieceOnPos = piece;
        if (pieceOnPos == null)
        {
            RecalcBlocked(target, to);
        }
        else
        {
            RecalcFree(target, to);

            if (autoLock)
            {
                RecalcFor(pieceOnPos, target);
            }
        }

        lastCheckedPosition = target;
    }

    private void RecalcBlocked(BoardPosition target, BoardPosition changedPosition)
    {
        foreach (var piece in blockPathPieces.Keys.ToList())
        {
            var blockers = blockPathPieces[piece];
            if (!blockers.Contains(changedPosition))
                continue;

            RecalcFor(piece, target);
        }
    }

    private void RecalcFree(BoardPosition target, BoardPosition changedPosition)
    {
        var current = 0;
        while (current < freePieces.Count)
        {
            var piece = freePieces[current];
            if (RecalcFor(piece, target))
                current++;
        }
    }

    protected virtual void RecalcAll(BoardPosition target)
    {
        lastCheckedPosition = target;
        
        var allPieces = blockPathPieces.Keys.ToList();
        allPieces.AddRange(freePieces);
        
        blockPathPieces.Clear();
        freePieces.Clear();
        
        foreach (var piece in allPieces)
        {
            RecalcFor(piece, target);
        }
    }
}