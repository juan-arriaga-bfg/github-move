using System.Collections.Generic;

public class PathfindIgnoreComponent:IgnoreComponent<Piece>
{
    protected readonly HashSet<int> ignorablePieceTypes;

    protected PathfindIgnoreComponent()
    {
        ignorablePieceTypes = new HashSet<int>();
    }
    
    public override bool CanIgnore(Piece item)
    {
        return item == null || ignorablePieceTypes.Contains(item.PieceType);
    }

    public PathfindIgnoreComponent(HashSet<int> ignorable)
    {
        ignorablePieceTypes = ignorable;
    }
    
    private static PathfindIgnoreComponent empty = new PathfindIgnoreComponent();
    public static PathfindIgnoreComponent Empty => empty;
}