using System.Collections.Generic;
using UnityEngine;

public class DraggablePieceComponent : ECSEntity, ILockerComponent
{
    public static int ComponentGuid = ECSManager.GetNextGuid();
    public override int Guid => ComponentGuid;

    private LockerComponent locker;
    public LockerComponent Locker => locker ?? GetComponent<LockerComponent>(LockerComponent.ComponentGuid);
    
    protected Piece context;
    
    public override void OnRegisterEntity(ECSEntity entity)
    {
        context = entity as Piece;
        
        locker = new LockerComponent();
        RegisterComponent(locker);
    }
    
    public virtual bool IsDraggable(BoardPosition at)
    {
        Debug.LogError($"IsDraggable check ({!Locker.IsLocked})");
        return !Locker.IsLocked;
    }
    
    public virtual bool IsValidDrag(BoardPosition to)
    {
        var board = context.Context;
        
        to.Z = board.BoardDef.PieceLayer;
        
        if (context.CachedPosition.Equals(to)) return true;
        
        if (to.IsValidFor(board.BoardDef.Width, board.BoardDef.Height) == false) return false;

        var targetPositions = GetAllPiecePoints(context, to);
        
        foreach (var position in targetPositions)
        {
            var pieceTo = board.BoardLogic.GetPieceAt(position);
            
            if (context.Equals(pieceTo))
                continue;
			
            if (board.BoardLogic.IsLockedCell(position))
                return false;

            if (pieceTo == null) continue;
            
            if (pieceTo.Draggable == null || !pieceTo.Draggable.IsDraggable(position) || pieceTo.Multicellular != null)
                return false;
        }
        
        return true;
    }
    
    private List<BoardPosition> GetAllPiecePoints(Piece piece, BoardPosition shift)
    {
        if (piece.Multicellular == null) return new List<BoardPosition> {shift};
        
        var result = new List<BoardPosition>();
        
        foreach (var point in piece.Multicellular.Mask)
        {
            result.Add(point + shift);
        }

        return result;
    }
}