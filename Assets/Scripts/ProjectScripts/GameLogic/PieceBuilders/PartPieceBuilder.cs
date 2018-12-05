﻿public class PartPieceBuilder : SimplePieceBuilder
{
    public override Piece Build(int pieceType, BoardController context)
    {
        var piece = base.Build(pieceType, context);
        var matcheble = piece.GetComponent<MatchablePieceComponent>(MatchablePieceComponent.ComponentGuid);
        
        matcheble?.Locker.Lock(piece);
        
        CreateViewComponent(piece);
        AddObserver(piece, new PartPieceBoardObserver());
        
        return piece;
    }
}