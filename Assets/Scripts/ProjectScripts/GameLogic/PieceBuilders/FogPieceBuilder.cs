﻿using System.Collections.Generic;

public class FogPieceBuilder : MulticellularPieceBuilder
{
    public override Piece Build(int pieceType, BoardController context)
    {
        var piece = base.Build(pieceType, context);
        
        
        
        piece.RegisterComponent(new TouchReactionComponent()
            .RegisterComponent(new TouchReactionDefinitionComponent())
            .RegisterComponent(new TouchReactionConditionFog()))
            .RegisterComponent(new FogPathfindBoardCondition(context, piece)
                .RegisterComponent(PathfindIgnoreBuilder.Build(piece.PieceType)));
        
        CreateViewComponent(piece);
        
        AddPathfindLockObserver(piece, true,  new List<LockerComponent>()
        {
            piece.TouchReaction.ReactionCondition.Locker
        });
        
        return piece;
    }

    protected override MulticellularPieceBoardObserver CreateMultiObserver()
    {
        return new FogObserver();
    }
}