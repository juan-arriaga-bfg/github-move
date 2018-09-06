using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Lock all cells around point
/// </summary>
public class AreaLockComponent : IECSComponent, IPieceBoardObserver
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();
    public int Guid => ComponentGuid;

    public List<BoardPosition> LockedCells;

    public const int RADIUS = 1;
    
    public void OnRegisterEntity(ECSEntity entity)
    {

    }

    public void OnUnRegisterEntity(ECSEntity entity)
    {

    }

    public void OnAddToBoard(BoardPosition position, Piece piece = null)
    {
        int size = RADIUS * 2 + 1;
        LockedCells = BoardPosition.GetRectInCenterForArea(position, size, size, piece.Context.BoardDef.Width, piece.Context.BoardDef.Height, false);
        piece.Context.BoardLogic.LockCells(LockedCells, this);

        // foreach (var cell in LockedCells)
        // {
        //     Debug.Log($"[AreaLockComponent] => Cell locked: {cell}");
        // }
    }
    
    public void OnRemoveFromBoard(BoardPosition position, Piece piece = null)
    {
        piece.Context.BoardLogic.UnlockCells(LockedCells, this);
    }

    public void OnMovedFromToStart(BoardPosition @from, BoardPosition to, Piece context = null)
    {

    }

    public void OnMovedFromToFinish(BoardPosition @from, BoardPosition to, Piece context = null)
    {

    }
}