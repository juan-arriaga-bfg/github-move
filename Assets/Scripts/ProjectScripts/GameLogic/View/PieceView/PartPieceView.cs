using UnityEngine;

public class PartPieceView : BuildingPieceView
{
    private bool isFake;
    private PartPieceBoardObserver observer;
    
    public override void Init(BoardRenderer context, Piece piece)
    {
        base.Init(context, piece);
        
        isFake = PieceType.GetDefById(Piece.PieceType).Filter.Has(PieceTypeFilter.Fake);
        observer = Piece.GetComponent<PartPieceBoardObserver>(PartPieceBoardObserver.ComponentGuid);
    }

    public override void OnDragStart(BoardPosition boardPos, Vector2 worldPos)
    {
        base.OnDragStart(boardPos, worldPos);

        if (isFake) return;
        
        Piece.Context.BoardLogic.CellHints.OnDragStart(Piece.CachedPosition, Piece.PieceType);
        Piece.Context.PartPiecesLogic.Remove(Piece.CachedPosition);
    }

    public override void OnDragEnd(BoardPosition boardPos, Vector2 worldPos)
    {
        if (isFake == false) Piece.Context.BoardLogic.CellHints.OnDragEnd();
        
        base.OnDragEnd(boardPos, worldPos);

        var position = new BoardPosition(boardPos.X, boardPos.Y, BoardLayer.Piece.Layer);

        if (Piece.CachedPosition.Equals(position) == false) return;

        observer?.AddBubble(position, Piece.PieceType);
    }
}