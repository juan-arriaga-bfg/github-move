using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Schema;
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
        return !blockPathPieces.ContainsKey(piece) && freePieces.Contains(piece); 
    }

    private List<LockerComponent> GetLockers(Piece piece)
    {
        var entities = piece.ComponentsCache.Values;
        var lockers = new List<LockerComponent>();
        foreach (var component in entities)
        {
            if (component is ILockerComponent)
            {
                var ILocker = component as ILockerComponent;
                var locker = ILocker.Locker;
                lockers.Add(locker);
            }
        }
            
        return lockers;
    }
    
    private void LockPathfinding(Piece piece)
    {
        if (blockPathPieces.ContainsKey(piece))
            return;

        piece.ActorView?.ToggleLockView(true);
        
        var lockers = GetLockers(piece);
        foreach (var lockerComponent in lockers)
        {
            lockerComponent.Lock(this);
        }
    }
    
    private void UnlockPathfinding(Piece piece)
    {
        if (piece.PieceType == PieceType.LockedEmpty.Id)
        {
            context.ActionExecutor.AddAction(new CollapsePieceToAction()
            {
                IsMatch = false,
                Positions = new List<BoardPosition>() {piece.CachedPosition},
                To = piece.CachedPosition
            });    
        }
        
        if (freePieces.Contains(piece))
            return;
        
        piece.ActorView?.ToggleLockView(false);
        
        var lockers = GetLockers(piece);
        foreach (var lockerComponent in lockers)
        {
            lockerComponent.Unlock(this);
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
            UnlockPathfinding(piece);
            blockPathPieces.Remove(piece);
            freePieces.Add(piece);
        }
        else if (canPath == false)
        {
            LockPathfinding(piece);
            freePieces.Remove(piece);
            blockPathPieces[piece] = pieceBlockers;
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