using System.Collections.Generic;

public class PiecePathfindBoardCondition : BoardConditionComponent
{
    private Piece piece;
    public Piece Piece
    {
        get { return piece; }
    }

    private PathfindIgnoreComponent pathfindIgnore;

    public PathfindIgnoreComponent PathfindIgnore
    {
        get
        {
            return pathfindIgnore
                   ?? (pathfindIgnore = GetComponent<PathfindIgnoreComponent>(PathfindIgnoreComponent.ComponentGuid))
                   ?? (pathfindIgnore = PathfindIgnoreComponent.Empty);
        }
    }

    public PiecePathfindBoardCondition(BoardController context, Piece piece) : base(context)
    {
        this.piece = piece;
    }

    public override bool Check(BoardPosition position)
    {
        var limits = CheckMapLimits(position);
        if (!limits)
            return false;

        var boardLogic = Context.BoardLogic;
        
        var pieceInCurrentPos = boardLogic.GetPieceAt(position);
        if (!boardLogic.IsLockedCell(position) && (pieceInCurrentPos == null || pieceInCurrentPos == Piece || PathfindIgnore.CanIgnore(pieceInCurrentPos)))
            return true;

        return false;
    }
}
