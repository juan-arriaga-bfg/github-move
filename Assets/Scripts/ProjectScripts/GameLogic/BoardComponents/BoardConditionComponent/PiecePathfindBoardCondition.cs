public class PiecePathfindBoardCondition : BoardConditionComponent
{
    private Piece piece;
    public Piece Piece => piece;

    private PathfindIgnoreComponent pathfindIgnore;

    public PathfindIgnoreComponent PathfindIgnore => pathfindIgnore
                                                     ?? (pathfindIgnore = GetComponent<PathfindIgnoreComponent>(PathfindIgnoreComponent.ComponentGuid))
                                                     ?? (pathfindIgnore = PathfindIgnoreComponent.Empty);

    public PiecePathfindBoardCondition(BoardController context, Piece piece) : base(context)
    {
        this.piece = piece;
    }

    public override bool Check(BoardPosition position)
    {
        var boardLogic = Context.BoardLogic;
        
        if (!CheckMapLimits(position) || boardLogic.IsLockedCell(position)) return false;
        
        var pieceInCurrentPos = boardLogic.GetPieceAt(position);
        
        return pieceInCurrentPos == Piece || PathfindIgnore.CanIgnore(pieceInCurrentPos);
    }
}
