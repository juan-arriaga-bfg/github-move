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

    private List<PathfindRegion> regions = new List<PathfindRegion>();
    
    public override void OnRegisterEntity(ECSEntity entity)
    {
        base.OnRegisterEntity(entity);
        context = entity as BoardController;
    }
    
    public virtual bool HasPath(Piece piece)
    {
        return !blockPathPieces.ContainsKey(piece); 
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

    public List<BoardPosition> GetBlockPathPositions(Piece lockedPiece)
    {
        List<BoardPosition> blockPath;
        if (blockPathPieces.TryGetValue(lockedPiece, out blockPath) == false)
        {
            return null;
        }

        return blockPath;
    }
    
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

    private void OpenPiece(Piece piece)
    {
        UnlockPathfinding(piece);
        blockPathPieces.Remove(piece);
        freePieces.Add(piece);
        
        piece.GetComponent<PathfindLockListenerComponent>(PathfindLockListenerComponent.ComponentGuid)?.UpdatePathState(true);
    }

    private void ClosePiece(Piece piece, List<BoardPosition> pieceBlockers)
    {
        LockPathfinding(piece);
        freePieces.Remove(piece);
        blockPathPieces[piece] = pieceBlockers;
        
        piece.GetComponent<PathfindLockListenerComponent>(PathfindLockListenerComponent.ComponentGuid)?.UpdatePathState(false);
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
            OpenPiece(piece);
        }
        else if (canPath == false)
        {
            ClosePiece(piece, pieceBlockers);
        }
        
        return canPath;
    }

    public virtual void RecalcCacheOnPieceAdded(HashSet<BoardPosition> target, BoardPosition changedPosition, Piece piece)
    {
        if (piece.PieceType == PieceType.Fog.Id)
        {
            RecalcFor(piece, target);
        }
    }
    
    private List<BoardPosition> FindConnections(BoardPosition at, List<BoardPosition> positions)
    {
        var result = new List<BoardPosition>();
        foreach (var pos in positions)
        {
            if (pos.IsNeighbor(at))
            {
                var neighPiece = context.BoardLogic.GetPieceAt(at);
                if(neighPiece!= null && PieceType.GetDefById(neighPiece.PieceType).Filter.HasFlag(PieceTypeFilter.Obstacle))
                    continue;
                result.Add(pos);
            }
        }

        return result;
    }
    
    private List<BoardPosition> CutRegion(List<BoardPosition> area)
    {
        
        
        var group = new HashSet<BoardPosition>();
        var uncheckedPositions = new HashSet<BoardPosition>();
        var firstPosition = area.First();
        uncheckedPositions.Add(firstPosition);
        
        while (uncheckedPositions.Count > 0)
        {
            var current = uncheckedPositions.First();
            var piece = context.BoardLogic.GetPieceAt(current);
            uncheckedPositions.Remove(current);
            
            var isObstacle = piece != null &&
                             PieceType.GetDefById(piece.PieceType).Filter.HasFlag(PieceTypeFilter.Obstacle);
            
            if(isObstacle && group.Count > 0)
                continue;
            
            area.Remove(current);
            
            if (isObstacle)
                return new List<BoardPosition>() {piece.CachedPosition};
                      
            
            group.Add(current);
            
            var connections = FindConnections(current, area);
            
            foreach (var connection in connections)
            {
                if (group.Contains(connection) == false && uncheckedPositions.Contains(connection) == false)
                    uncheckedPositions.Add(connection);
            }
        }

        return group.ToList();
    }
    
    private List<PathfindRegion> GetRegionsByPositions(List<BoardPosition> area)
    {
        var lockedArea = new List<BoardPosition>();
        foreach (var pos in area)
        {
            var piecePos = new BoardPosition(pos.X, pos.Y, BoardLayer.Piece.Layer);
            var piece = context.BoardLogic.GetPieceAt(piecePos);
            
            if(piece != null && piece.PieceType == PieceType.Fog.Id || context.BoardLogic.IsLockedCell(piecePos))
                continue;
            lockedArea.Add(piecePos);
        }
        var regions = new List<PathfindRegion>();
        var currentRegionPositions = CutRegion(lockedArea);
        while (currentRegionPositions.Count > 0)
        {
            
            var region = new PathfindRegion(context);
            foreach (var pos in currentRegionPositions)
            {
                region.AddPosition(pos);
            }
            regions.Add(region);
            currentRegionPositions = lockedArea.Count > 0 ? CutRegion(lockedArea) : new List<BoardPosition>();
        }
        return regions;
    }

    private void RecalculateRegions(Piece changedPiece)
    {
        Action<HashSet<Piece>> onRegionOpen = (pieces) =>
        {
            foreach (var piece in pieces)
            {
                OpenPiece(piece);
            }
        };
        var regionsForRemove = new HashSet<PathfindRegion>();
        foreach (var region in regions)
        {
            if(region.RecalculateState(onRegionOpen, changedPiece))
                regionsForRemove.Add(region);
        }
        
        regions.RemoveAll(elem => regionsForRemove.Contains(elem));
    }

    public void DebugFullRecalculate()
    {
        OnAddComplete(BoardPosition.GetRect(BoardPosition.Zero(), context.BoardDef.Width, context.BoardDef.Height));
    }
    
    public void OnAddComplete(List<BoardPosition> spawnArea)
    {
        var newRegions = GetRegionsByPositions(spawnArea);
        Action<HashSet<Piece>> onRegionOpen = (pieces) =>
        {
            foreach (var piece in pieces)
            {
                OpenPiece(piece);
            }
        };
        
        foreach (var region in newRegions)
        {
            if (region.RecalculateState(onRegionOpen) == false)
            {
                foreach (var regPiece in region.RegionPieces)
                {
                    ClosePiece(regPiece, region.BlockPathPieces);
                }
                regions.Add(region);
            }   
        }
    }
    
    public virtual void RecalcCacheOnPieceRemoved(Piece removedPiece)
    {
        if (removedPiece.PieceType == PieceType.Fog.Id)
        {
            OnFogRemove(removedPiece);
        }
        
        RecalculateRegions(removedPiece);
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
                        To = emptyCell.CachedPosition,
                        AnimationResourceSearch = piece => AnimationOverrideDataService.Current.FindAnimation(piece, def => def.OnDestroyFromBoard)
                    });
                }
            }
        }
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
                if (piece.PieceType != PieceType.Fog.Id)
                    RecalcFor(pieceOnPos, target);
            }
        }
    }

    private void RecalcBlocked(HashSet<BoardPosition> target, BoardPosition changedPosition)
    {
        var changedPiece = BoardService.Current.FirstBoard.BoardLogic.GetPieceAt(changedPosition);
        RecalculateRegions(changedPiece);
    }

    private void RecalcFree(HashSet<BoardPosition> target)
    {
        RecalculateRegions(null);
    }

    public virtual void RecalcAll(HashSet<BoardPosition> target)
    {
        var allPieces = blockPathPieces.Keys.ToList();
        allPieces.AddRange(freePieces);
        
        blockPathPieces.Clear();
        freePieces.Clear();
        
        foreach (var piece in allPieces)
        {
            if (piece.PieceType != PieceType.Fog.Id)
                RecalcFor(piece, target);
        }
    }

    private List<BoardPosition> GetOuterBorderPositions(HashSet<BoardPosition> area)
    {
        var border = new HashSet<BoardPosition>();
        foreach (var pos in area)
        {
            
            var neighbors = pos.Neighbors();
            foreach (var position in pos.Neighbors())
            {
                var tileDefId = GameDataService.Current.FieldManager.GetTileId(position.X, position.Y);
                var isRelief = tileDefId != BoardTiles.WATER_TILE_ID && BoardTiles.GetDefs()[tileDefId].IsLock;
                if (isRelief)
                {
                    neighbors.AddRange(position.Neighbors());
                }
            }
            foreach (var neighPos in neighbors)
            {
                var normilizedNeight = new BoardPosition(neighPos.X, neighPos.Y, BoardLayer.Piece.Layer);
                if (area.Contains(normilizedNeight) == false && border.Contains(normilizedNeight) == false)
                    border.Add(normilizedNeight);
            }
        }

        return border.ToList();
    }
    
    private List<Piece> GetNearFogs(Piece fog)
    {
        var mask = fog.Multicellular.Mask;
        var fogPositions = new HashSet<BoardPosition>();
        foreach (var maskPos in mask)
        {
            fogPositions.Add(maskPos);
        }

        var outBorder = GetOuterBorderPositions(fogPositions);
        
        var nearFogs = new List<Piece>();
        foreach (var outPos in outBorder)
        {
            var piece = context.BoardLogic.GetPieceAt(outPos);
            if (piece != null && piece.PieceType == PieceType.Fog.Id && nearFogs.Contains(piece) == false)
            {
                
                nearFogs.Add(piece);
            }             
        }
        return nearFogs;
        
    }
    
    private void OnFogRemove(Piece fog)
    {
        var nearFogs = GetNearFogs(fog);
        
        foreach (var nearFog in nearFogs)
            if (freePieces.Contains(nearFog) == false)
                OpenPiece(nearFog);
    }
}