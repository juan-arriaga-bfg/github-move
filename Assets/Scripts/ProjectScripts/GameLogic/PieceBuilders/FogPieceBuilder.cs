﻿public class FogPieceBuilder : MulticellularPieceBuilder
{
    public override Piece Build(int pieceType, BoardController context)
    {
        var piece = base.Build(pieceType, context);
        
        piece.RegisterComponent(new TouchReactionComponent()
            .RegisterComponent(new TouchReactionDefinitionComponent())
            .RegisterComponent(new TouchReactionConditionFog()));
        
        CreateViewComponent(piece);
        
        return piece;
    }

    protected override MulticellularPieceBoardObserver CreateMultiObserver()
    {
        return new FogObserver();
    }
}