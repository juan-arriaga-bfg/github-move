﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PathfindLockerComponent : ECSEntity
{
    private class Region
    {
        private HashSet<BoardPosition> regPositions;
        
        private HashSet<Piece> piecesOnRegion;
        public HashSet<Piece> RegionPieces => piecesOnRegion;
        
        private List<BoardPosition> blockPathPositions;

        private BoardController board;
        
        public bool Contains(BoardPosition position)
        {
            return regPositions.Contains(position);
        }

        public void AddPosition(BoardPosition position)
        {
            regPositions.Add(position);
            
            var pieceOnPos = board.BoardLogic.GetPieceAt(position);
            if (pieceOnPos != null && piecesOnRegion.Contains(pieceOnPos) == false)
                piecesOnRegion.Add(pieceOnPos);
        }

        public void RemovePosition(BoardPosition position)
        {
            regPositions.Remove(position);
            
            var pieceOnPos = board.BoardLogic.GetPieceAt(position);
            if (pieceOnPos != null && piecesOnRegion.Contains(pieceOnPos))
                piecesOnRegion.Remove(pieceOnPos);
        }

        

        public bool RecalculateState(Action<HashSet<Piece>> onRegionOpen, Piece changedPiece = null)
        {
            if (changedPiece != null &&
                piecesOnRegion.Contains(changedPiece) &&
                board.BoardLogic.GetPieceAt(changedPiece.CachedPosition) == null)
                piecesOnRegion.Remove(changedPiece);
            
            //recalculate path
            if (piecesOnRegion.Count == 0 || (changedPiece != null && blockPathPositions.Contains(changedPiece.CachedPosition) == false))
                return false;
            var firstPiece = piecesOnRegion.First();
            var canPath = board.Pathfinder.HasPath(firstPiece.CachedPosition, board.AreaAccessController.AvailiablePositions, 
                                                   out blockPathPositions, firstPiece, board.Pathfinder.GetCondition(firstPiece));
            if (canPath)
            {
                onRegionOpen?.Invoke(piecesOnRegion);
            }
                   
            return canPath;
        }
        
        public Region(BoardController boardController)
        {
            board = boardController;
            regPositions = new HashSet<BoardPosition>();
            blockPathPositions = new List<BoardPosition>();
            piecesOnRegion = new HashSet<Piece>();
        }
    }
    
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();
    public override int Guid => ComponentGuid;

    private BoardController context;

    private PathfinderComponent pathfinder;
    public PathfinderComponent Pathfinder => pathfinder ??
                                             (pathfinder = context?.Pathfinder);

    private Dictionary<Piece, List<BoardPosition>> blockPathPieces = new Dictionary<Piece, List<BoardPosition>>();
    private List<Piece> freePieces = new List<Piece>();

    private List<Region> regions = new List<Region>();
    
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

    private void OpenPiece(Piece piece)
    {
        UnlockPathfinding(piece);
        blockPathPieces.Remove(piece);
        freePieces.Add(piece);
    }

    private void ClosePiece(Piece piece, List<BoardPosition> pieceBlockers)
    {
        LockPathfinding(piece);
        freePieces.Remove(piece);
        blockPathPieces[piece] = pieceBlockers;
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

    private List<Piece> pieceAddCache = new List<Piece>();
    public virtual void RecalcCacheOnPieceAdded(HashSet<BoardPosition> target, BoardPosition changedPosition, Piece piece)
    {
        if (piece.PieceType == PieceType.Fog.Id)
            RecalcFor(piece, target);
        if (piece.PathfindLockObserver.AutoLock)
            pieceAddCache.Add(piece);     
    }
    
    private List<BoardPosition> FindConnections(BoardPosition at, List<BoardPosition> positions)
    {
        var piece = context.BoardLogic.GetPieceAt(at);
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
        uncheckedPositions.Add(area.First());
        
        while (uncheckedPositions.Count > 0)
        {
            var current = uncheckedPositions.First();
            var piece = context.BoardLogic.GetPieceAt(current);
            uncheckedPositions.Remove(current);
            area.Remove(current);
            var isObstacle = piece != null &&
                             PieceType.GetDefById(piece.PieceType).Filter.HasFlag(PieceTypeFilter.Obstacle);
            if (isObstacle && group.Count == 0)
                return new List<BoardPosition>() {piece.CachedPosition};
            if(isObstacle)
                continue;            
            
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
    
    private List<Region> GetRegionsByPositions(List<BoardPosition> lockedArea)
    {
        var regions = new List<Region>();
        var currentRegionPositions = CutRegion(lockedArea);
        while (currentRegionPositions.Count > 0)
        {
            Debug.LogError($"ITERATION");
            var region = new Region(context);
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
        var regionsForRemove = new HashSet<Region>();
        foreach (var region in regions)
        {
            if(region.RecalculateState(onRegionOpen, changedPiece))
                regionsForRemove.Add(region);
        }

        regions.RemoveAll(elem => regionsForRemove.Contains(elem));
        Debug.LogError("Recalculate regions");
    }
    
    public void OnAddComplete(List<BoardPosition> spawnArea)
    {
        Debug.LogError($"AddComplete on area: {string.Join(",", spawnArea)}");
        //var target = context.AreaAccessController.AvailiablePositions;

        var newRegions = GetRegionsByPositions(spawnArea);
        Action<HashSet<Piece>> onRegionOpen = (pieces) =>
        {
            foreach (var piece in pieces)
            {
                OpenPiece(piece);
            }
        };
        Debug.LogError($"Region count: {newRegions.Count}");
        foreach (var region in newRegions)
        {
            Debug.LogError($"Region contains: {string.Join(",", region.RegionPieces.Select(elem => elem.CachedPosition))}");
            if (region.RecalculateState(onRegionOpen) == false)
            {
                foreach (var regPiece in region.RegionPieces)
                {
                    ClosePiece(regPiece, new List<BoardPosition>());
                }
                regions.Add(region);
            }   
        }
    }
    
    public virtual void RecalcCacheOnPieceRemoved(HashSet<BoardPosition> target , BoardPosition changedPosition, Piece removedPiece)
    {
        if(removedPiece.PieceType == PieceType.Fog.Id)
            OnFogRemove(removedPiece);
        
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
                if (piece.PieceType != PieceType.Fog.Id)
                    RecalcFor(pieceOnPos, target);
            }
        }
    }

    private void RecalcBlocked(HashSet<BoardPosition> target, BoardPosition changedPosition)
    {
        
        var changedPositions = new List<BoardPosition>();

        var changedPiece = BoardService.Current.FirstBoard.BoardLogic.GetPieceAt(changedPosition);
        RecalculateRegions(changedPiece);
        return;
        changedPositions.Add(changedPosition);
       

        foreach (var piece in blockPathPieces.Keys.ToList())
        {
            const int maxCheckDistance = 100;
            var blockers = blockPathPieces[piece];
            if (blockers.Any(elem => changedPositions.Contains(elem)) 
                || Math.Abs((piece.CachedPosition.X + piece.CachedPosition.Y) - (changedPosition.X + changedPosition.Y)) < maxCheckDistance)
                if (piece.PieceType != PieceType.Fog.Id)
                    RecalcFor(piece, target, new List<BoardPosition>(target));
            
        }
    }

    private void RecalcFree(HashSet<BoardPosition> target)
    {
        RecalculateRegions(null);
        return;
        var current = 0;
        while (current < freePieces.Count)
        {
            var piece = freePieces[current];
            if ((piece.PieceType == PieceType.Fog.Id) || RecalcFor(piece, target, new List<BoardPosition>(target)))
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
            foreach (var neighPos in neighbors)
            {
                var normilizedNeight = new BoardPosition(neighPos.X, neighPos.Y, context.BoardDef.PieceLayer);
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