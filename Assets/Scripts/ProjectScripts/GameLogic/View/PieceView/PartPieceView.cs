using UnityEngine;

public class PartPieceView : BuildingPieceView
{
    private bool isFake;
    
    public override void Init(BoardRenderer context, Piece piece)
    {
        base.Init(context, piece);
        isFake = PieceType.GetDefById(Piece.PieceType).Filter.Has(PieceTypeFilter.Fake);
    }

    public override void OnDragStart(BoardPosition boardPos, Vector2 worldPos)
    {
        base.OnDragStart(boardPos, worldPos);
        
        if(isFake) return;
        
        Piece.Context.BoardLogic.CellHints.OnDragStart(Piece.CachedPosition, Piece.PieceType);
    }

    public override void OnDragEnd(BoardPosition boardPos, Vector2 worldPos)
    {
        if(isFake == false) Piece.Context.BoardLogic.CellHints.OnDragEnd();
        base.OnDragEnd(boardPos, worldPos);
    }
}