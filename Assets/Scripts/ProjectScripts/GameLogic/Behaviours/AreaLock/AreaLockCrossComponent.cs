using System.Collections.Generic;

/// <summary>
/// Lock 1 cell at t b l r around point
/// </summary>
public class AreaLockCrossComponent : IECSComponent, IPieceBoardObserver
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();
    public int Guid => ComponentGuid;

    public List<BoardPosition> LockedCells;

    public void OnRegisterEntity(ECSEntity entity)
    {

    }

    public void OnUnRegisterEntity(ECSEntity entity)
    {

    }

    public void OnAddToBoard(BoardPosition position, Piece piece = null)
    {
        LockedCells = position.NeighborsFor(piece.Context.BoardDef.Width, piece.Context.BoardDef.Height);
        piece.Context.BoardLogic.LockCells(LockedCells, this);
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