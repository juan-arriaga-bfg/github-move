public class MineDraggablePieceComponent : DraggablePieceComponent
{
    public override bool IsDraggable(BoardPosition at)
    {
        return base.IsDraggable(at) && context.Pathfinder.CanPathToCastle(context);
    }

    public override bool IsValidDrag(BoardPosition to)
    {
        return base.IsValidDrag(to) && context.Pathfinder.CanPathToCastle(context, to);
    }
}