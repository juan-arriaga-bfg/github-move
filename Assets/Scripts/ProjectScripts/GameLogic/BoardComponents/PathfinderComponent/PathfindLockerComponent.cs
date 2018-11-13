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

    private Stack<Piece> unlockedEmptyPieces = new Stack<Piece>();

    public List<Piece> CollectUnlockedEmptyCells()
    {
        var collapseEmptyPieces = unlockedEmptyPieces.ToList();
        unlockedEmptyPieces.Clear();
        return collapseEmptyPieces;    
    }

    private void UnlockPathfinding(Piece piece)
    {
        if (piece.PieceType == PieceType.LockedEmpty.Id && unlockedEmptyPieces.Contains(piece) == false)
        {
            unlockedEmptyPieces.Push(piece);
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
        
        List<BoardPosition> pieceBlockers = new List<BoardPosition>();
        
        var defaultCondition = Pathfinder.GetCondition(piece);
        Predicate<BoardPosition> pathCondition = (pos) => ignorablePositions.Contains(pos) || defaultCondition(pos);
        
        bool canPath = context.AreaAccessController.AvailiablePositions.Contains(piece.CachedPosition);
        
        if(canPath == false)
            canPath = Pathfinder.HasPath(piece.CachedPosition, target, out pieceBlockers, piece, pathCondition);

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

    private List<Piece> pieceAddCache = new List<Piece>();
    public virtual void RecalcCacheOnPieceAdded(HashSet<BoardPosition> target, BoardPosition changedPosition, Piece piece)
    {   
        if (piece.PathfindLockObserver.AutoLock)
            pieceAddCache.Add(piece);     
    }

    public void OnAddComplete()
    {
        var target = context.AreaAccessController.AvailiablePositions;

        foreach (var piece in pieceAddCache)
        {
            RecalcFor(piece, target);
        }
        pieceAddCache.Clear();
    }
    
    public virtual void RecalcCacheOnPieceRemoved(HashSet<BoardPosition> target , BoardPosition changedPosition, Piece removedPiece)
    {
        RecalcBlocked(target, removedPiece.CachedPosition);
        if (PieceType.GetDefById(removedPiece.PieceType).Filter.HasFlag(PieceTypeFilter.Obstacle))
        {
            var emptyCells = CollectUnlockedEmptyCells();
            foreach (var emptyCell in emptyCells)
            {
                var hasPath = HasPath(emptyCell);
                if (hasPath)
                {
                    context.ActionExecutor.AddAction(new CollapsePieceToAction()
                    {
                        IsMatch = false,
                        Positions = new List<BoardPosition>() {emptyCell.CachedPosition},
                        To = emptyCell.CachedPosition
                    });
                }
            }
        }
        var board = context;
    }

    public virtual void RemoveFromCache(Piece piece)
    {
        freePieces.Remove(piece);
        blockPathPieces.Remove(piece);
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
            RecalcFree(target);

            if (autoLock)
            {
                RecalcFor(pieceOnPos, target);
            }
        }
    }

    private void RecalcBlocked(HashSet<BoardPosition> target, BoardPosition changedPosition)
    {
        var changedPositions = new List<BoardPosition>();

        var changedPiece = BoardService.Current.FirstBoard.BoardLogic.GetPieceAt(changedPosition);
        changedPositions.Add(changedPosition);
       

        foreach (var piece in blockPathPieces.Keys.ToList())
        {
            const int maxCheckDistance = 100;
            var blockers = blockPathPieces[piece];
            if (blockers.Any(elem => changedPositions.Contains(elem)) 
                || Math.Abs((piece.CachedPosition.X + piece.CachedPosition.Y) - (changedPosition.X + changedPosition.Y)) < maxCheckDistance)
                RecalcFor(piece, target, new List<BoardPosition>(target));
            
        }
    }

    private void RecalcFree(HashSet<BoardPosition> target)
    {
        var current = 0;
        while (current < freePieces.Count)
        {
            var piece = freePieces[current];
            if (RecalcFor(piece, target, new List<BoardPosition>(target)))
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