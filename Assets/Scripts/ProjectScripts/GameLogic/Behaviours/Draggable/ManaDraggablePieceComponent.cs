public class ManaDraggablePieceComponent : DraggablePieceComponent
{
    public override bool IsValidDrag(BoardPosition to)
    {
        var board = context.Context;
        
        to.Z = BoardLayer.Piece.Layer;
        
        if (context.CachedPosition.Equals(to)) return true;
        
        if (to.IsValidFor(board.BoardDef.Width, board.BoardDef.Height) == false || board.BoardLogic.IsLockedCell(to)) return false;
        
        var target = board.BoardLogic.GetPieceAt(to);

        if (target == null) return true;
        
        if (!target.Context.PathfindLocker.HasPath(target)) return false;

        if (target.PieceType != PieceType.Fog.Id) return target.Draggable != null && target.Draggable.IsDraggable(to) && target.Multicellular == null;
        
        var fog = target.GetComponent<FogObserver>(FogObserver.ComponentGuid);
            
        return fog != null && fog.CanBeReached() && fog.RequiredLevelReached();
    }
}
