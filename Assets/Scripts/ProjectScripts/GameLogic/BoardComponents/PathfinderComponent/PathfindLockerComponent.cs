﻿using System;
using System.Collections.Generic;
using System.Linq;

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
    private HashSet<BoardPosition> lastCheckedPositions = new HashSet<BoardPosition>();
    
    public override void OnRegisterEntity(ECSEntity entity)
    {
        base.OnRegisterEntity(entity);
        context = entity as BoardController;
    }
    
    public virtual bool HasPath(Piece piece)
    {
        return !blockPathPieces.ContainsKey(piece) && freePieces.Contains(piece); 
    }

    public virtual void RecalcCacheOnPieceAdded(HashSet<BoardPosition> target, BoardPosition changedPosition, Piece piece, bool autoLock)
    {
        lastCheckedPositions = target;
        
        RecalcFree(target, changedPosition);

        
        if (autoLock)
        {
            var addedPiece = piece;
            RecalcFor(addedPiece, target);
        }   
    }

    private bool RecalcFor(Piece piece, HashSet<BoardPosition> target, List<BoardPosition> ignorablePositions = null)
    {
        if (ignorablePositions == null)
            ignorablePositions = new List<BoardPosition>();
        
        List<BoardPosition> pieceBlockers;
        
        var defaultCondition = Pathfinder.GetCondition(piece);
        Predicate<BoardPosition> pathCondition = (pos) => ignorablePositions.Contains(pos) || defaultCondition(pos);
        
        var canPath = Pathfinder.HasPath(piece.CachedPosition, target, out pieceBlockers, piece, pathCondition);
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
        
        return canPath;
    }
    
    public virtual void RecalcCacheOnPieceRemoved(HashSet<BoardPosition> target , BoardPosition changedPosition, Piece removedPiece)
    {
        if (changedPosition.Equals(lastCheckedPositions))
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

    public virtual void RecalcCacheOnPieceMoved(HashSet<BoardPosition> target, BoardPosition fromPosition, BoardPosition to, Piece piece,
        bool autoLock)
    {
        if (fromPosition.Equals(lastCheckedPositions))
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

        lastCheckedPositions = target;
    }

    private void RecalcBlocked(HashSet<BoardPosition> target, BoardPosition changedPosition)
    {
        foreach (var piece in blockPathPieces.Keys.ToList())
        {
            var blockers = blockPathPieces[piece];
            if (!blockers.Contains(changedPosition))
                continue;

            RecalcFor(piece, target, new List<BoardPosition>(target) {changedPosition});
        }
    }

    private void RecalcFree(HashSet<BoardPosition> target, BoardPosition changedPosition)
    {
        var current = 0;
        while (current < freePieces.Count)
        {
            var piece = freePieces[current];
            if (RecalcFor(piece, target))
                current++;
        }
    }

    protected virtual void RecalcAll(HashSet<BoardPosition> target)
    {
        lastCheckedPositions = target;
        
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