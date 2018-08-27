public class MineDraggablePieceComponent : DraggablePieceComponent
{
    public override bool IsDraggable(BoardPosition at)
    {
        return base.IsDraggable(at) && context.Context?.Pathfinder.CanPathToCastle(context) == true;
    }

    public override bool IsValidDrag(BoardPosition to)
    {
        return base.IsValidDrag(to) && context.Context?.Pathfinder.CanPathToCastle(context, to) == true;
    }
}