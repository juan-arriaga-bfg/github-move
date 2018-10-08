using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PathfindLockObserver: IECSComponent, IPieceBoardObserver
{
    public static int ComponentGuid = ECSManager.GetNextGuid();
    public int Guid => ComponentGuid;

    protected Piece piece;
    protected BoardController board;

    public readonly List<LockerComponent> Lockers;

    public PathfindLockObserver(params LockerComponent[] lockers)
    {
        Lockers = lockers.ToList();
    }
    
    private static List<PathfindLockObserver> nonLoaded = new List<PathfindLockObserver>();

    public static void LoadPathfindLock()
    {
        foreach (var pathfindLockObserver in nonLoaded)
        {
            var target = pathfindLockObserver.GetTargetPositions();
            if(target == null)
                continue;
            pathfindLockObserver.OnAddToBoard(pathfindLockObserver.piece.CachedPosition, pathfindLockObserver.piece);
        }
        nonLoaded.Clear();
    }

    public bool AutoLock = true;
    
    public virtual void OnRegisterEntity(ECSEntity entity)
    {
        piece = entity as Piece;
        board = piece.Context;
    }

    public virtual void OnUnRegisterEntity(ECSEntity entity)
    {
    }

    public void OnAddToBoard(BoardPosition position, Piece context = null)
    {
       if (piece == null || board == null)
       {
            board = BoardService.Current.GetBoardById(0);
            piece = board.BoardLogic.GetPieceAt(position);
       }
        
       var target = GetTargetPositions();
       if(target != null)
           board.PathfindLocker?.RecalcCacheOnPieceAdded(target, piece.CachedPosition, piece, AutoLock);
       else if(AutoLock)
           nonLoaded.Add(this);
    }

    public void OnMovedFromToStart(BoardPosition @from, BoardPosition to, Piece context = null)
    {
    }

    public void OnMovedFromToFinish(BoardPosition from, BoardPosition to, Piece context = null)
    {
        var target = GetTargetPositions();
        if(target != null)
            board.PathfindLocker?.RecalcCacheOnPieceMoved(target, from, to, piece, AutoLock);
    }

    public void OnRemoveFromBoard(BoardPosition position, Piece context = null)
    {
        var target = GetTargetPositions();
        if(target != null)
            board.PathfindLocker?.RecalcCacheOnPieceRemoved(target, position, piece);
    }

    private HashSet<BoardPosition> GetTargetPositions()
    {
        return board.AreaAccessController?.AvailiablePositions;
    }
}