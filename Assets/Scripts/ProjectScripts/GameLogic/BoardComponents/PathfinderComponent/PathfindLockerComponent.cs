using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
    
    public override void OnRegisterEntity(ECSEntity entity)
    {
        base.OnRegisterEntity(entity);
        context = entity as BoardController;
    }
    
    public virtual bool HasPath(Piece piece)
    {
        Debug.LogWarning($"{piece.CachedPosition} check path. Block contains: {blockPathPieces.ContainsKey(piece)}, free contains {freePieces.Contains(piece)}");
        return !blockPathPieces.ContainsKey(piece) && freePieces.Contains(piece); 
    }

    private void LockPathfinding(Piece piece)
    {
        var observer = piece.PathfindLockObserver;
        if (observer != null)
        {
            foreach (var lockerComponent in observer.Lockers)
            {
                lockerComponent.Lock(this);
            }
        }
    }

    private void UnlockPathfinding(Piece piece)
    {
        var observer = piece.PathfindLockObserver;
        if (observer != null)
        {
            foreach (var lockerComponent in observer.Lockers)
            {
                lockerComponent.Unlock(this);
            }
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
            UnlockPathfinding(piece);
            
        }
        else if (canPath == false)
        {
            freePieces.Remove(piece);
            if(blockPathPieces.ContainsKey(piece) == false)
                 LockPathfinding(piece);
            blockPathPieces[piece] = pieceBlockers;
            if (piece.CachedPosition.Equals(new BoardPosition(15, 14, 1)))
            {
                Debug.LogError($"{piece.CachedPosition} blocked by {string.Join(",", blockPathPieces[piece])}");
                foreach (var pos in blockPathPieces[piece])
                {
                    DevTools.Instance.MarkCell(pos);
                    Debug.LogError($"{pos} lock {piece.CachedPosition}. Type = {PieceType.GetDefById(context.BoardLogic.GetPieceAt(pos).PieceType).Abbreviations.First()}");
                }
            }
                
            
        }
        
        
        
        return canPath;
    }
    
    public virtual void RecalcCacheOnPieceAdded(HashSet<BoardPosition> target, BoardPosition changedPosition, Piece piece, bool autoLock)
    {    
        RecalcFree(target, changedPosition);

        if (autoLock)
        {
            var addedPiece = piece;
            RecalcFor(addedPiece, target);
        }   
    }
    
    public virtual void RecalcCacheOnPieceRemoved(HashSet<BoardPosition> target , BoardPosition changedPosition, Piece removedPiece)
    {
        Debug.LogError($"PathfindLocker execute on cell {removedPiece.CachedPosition}");
        RecalcBlocked(target, removedPiece.CachedPosition);
        
        if (freePieces.Contains(removedPiece))
            freePieces.Remove(removedPiece);
        if (blockPathPieces.ContainsKey(removedPiece))
            blockPathPieces.Remove(removedPiece);
    }

    public virtual void RecalcCacheOnPieceMoved(HashSet<BoardPosition> target, BoardPosition fromPosition, BoardPosition to, Piece piece,
        bool autoLock)
    {
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

    }

    private void RecalcBlocked(HashSet<BoardPosition> target, BoardPosition changedPosition)
    {
        var changedPositions = new List<BoardPosition>();
        var sourcePiece = context.BoardLogic.GetPieceAt(changedPosition);
        
        if (sourcePiece?.Multicellular == null)
            changedPositions.Add(changedPosition);
        else
            foreach (var maskItem in sourcePiece.Multicellular.Mask)
                changedPositions.Add(sourcePiece.Multicellular.GetPointInMask(sourcePiece.CachedPosition, maskItem));

        foreach (var piece in blockPathPieces.Keys.ToList())
        {
            var blockers = blockPathPieces[piece];
            if (!blockers.Any(elem => changedPositions.Contains(elem)))
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
            if (RecalcFor(piece, target, new List<BoardPosition>(target) {changedPosition}))
                current++;
        }
    }

    public virtual void RecalcAll(HashSet<BoardPosition> target)
    {
        
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