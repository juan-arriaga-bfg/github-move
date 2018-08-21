using System.Collections.Generic;

public class DraggablePieceComponent : IECSComponent
{
    public static int ComponentGuid = ECSManager.GetNextGuid();
    public int Guid => ComponentGuid;

    protected Piece context;
    
    public virtual void OnRegisterEntity(ECSEntity entity)
    {
        context = entity as Piece;
    }

    public virtual void OnUnRegisterEntity(ECSEntity entity)
    {
    }
    
    public virtual bool IsDraggable(BoardPosition at)
    {
        return true;
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