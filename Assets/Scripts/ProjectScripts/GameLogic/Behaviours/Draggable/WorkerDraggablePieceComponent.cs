public class WorkerDraggablePieceComponent : DraggablePieceComponent
{
    public override bool IsValidDrag(BoardPosition to)
    {
        var board = context.Context;
        
        to.Z = board.BoardDef.PieceLayer;
        
        if (context.CachedPosition.Equals(to)) return true;
        
        if (to.IsValidFor(board.BoardDef.Width, board.BoardDef.Height) == false || board.BoardLogic.IsLockedCell(to)) return false;
        
        var target = board.BoardLogic.GetPieceAt(to);

        if (target == null) return true;
        
        var reaction = target.GetComponent<PiecePathfindBoardCondition>(PiecePathfindBoardCondition.ComponentGuid);

        if (reaction != null)
        {
            return !reaction.Check(target.CachedPosition);
        }
        
        var life = target.GetComponent<StorageLifeComponent>(StorageLifeComponent.ComponentGuid);

        if (life != null)
        {
            return !life.Locker.IsLocked && (!life.IsUseCooldown || !life.Timer.IsExecuteable());
        }
        
        var state = target.GetComponent<PieceStateComponent>(PieceStateComponent.ComponentGuid);

        if (state != null)
        {
            return state.State != BuildingState.InProgress && state.State != BuildingState.Complete;
        }
        
        return target.Draggable != null && target.Draggable.IsDraggable(to) && target.Multicellular == null;
    }
}