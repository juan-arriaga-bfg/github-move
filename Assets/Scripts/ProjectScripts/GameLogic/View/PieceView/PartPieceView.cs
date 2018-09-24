using UnityEngine;

public class PartPieceView : BuildingPieceView
{
    public override void OnDragStart(BoardPosition boardPos, Vector2 worldPos)
    {
        base.OnDragStart(boardPos, worldPos);
        Piece.Context.BoardLogic.CellHints.OnDragStart(Piece.CachedPosition, Piece.PieceType);
    }

    public override void OnDragEnd(BoardPosition boardPos, Vector2 worldPos)
    {
        Piece.Context.BoardLogic.CellHints.OnDragEnd();
        base.OnDragEnd(boardPos, worldPos);
    }
}