public class MineDraggablePieceComponent : DraggablePieceComponent
{
    private Piece context;
    private PathfinderComponent pathfinder;

    public PathfinderComponent Pathfinder
    {
        get
        {
            return pathfinder ??
                   (pathfinder = context.GetComponent<PathfinderComponent>(PathfinderComponent.ComponentGuid));
        }
        set { pathfinder = value; }
    }
    
    public override void OnRegisterEntity(ECSEntity entity)
    {
        base.OnRegisterEntity(entity);
        context = entity as Piece;
    }

    public override bool IsDraggable(BoardPosition at)
    {
        if(!base.IsDraggable(at))
            return false;

        return Pathfinder.CanPathToCastle(context);
    }

    public override bool IsValidDrag(BoardPosition to)
    {
        if (!base.IsValidDrag(to))
            return false;

        var canPath = Pathfinder.CanPathToCastle(context, to);
        //if(!canPath)
            //SUIErrorWindowController.AddError("Place not in reach");
        return canPath;
    }
}