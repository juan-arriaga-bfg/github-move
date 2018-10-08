using System.Collections.Generic;
using UnityEngine;

public class SimplePieceBuilder : GenericPieceBuilder 
{
    public override Piece Build(int pieceType, BoardController context)
    {
        var piece = base.Build(pieceType, context);
        
        piece.RegisterComponent(new DraggablePieceComponent());
        
        var def = GameDataService.Current.PiecesManager.GetPieceDef(pieceType);

        if (def == null) return piece;

        if (def.ReproductionDef?.Reproduction != null)
        {
            var observer = new ReproductionPieceComponent {Child = def.ReproductionDef.Reproduction};
        
            piece.RegisterComponent(observer);
            AddObserver(piece, observer);
        }

        var pathfindLockers = new List<LockerComponent>() {piece.Draggable.Locker};

        piece.RegisterComponent(
            new PiecePathfindBoardCondition(piece.Context, piece)
                .RegisterComponent(PathfindIgnoreBuilder.Build(piece.PieceType
        )));
        
        if (def.SpawnResources != null)
        {
            piece.RegisterComponent(new ResourceStorageComponent {Resources = def.SpawnResources});

            piece.RegisterComponent(new TouchReactionComponent()
                .RegisterComponent(new TouchReactionDefinitionCollectResource())
                .RegisterComponent(new TouchReactionConditionComponent()));
            
            pathfindLockers.Add(piece.TouchReaction.Locker);
        }

        AddPathfindLockObserver(piece, true, pathfindLockers);
        
        return piece;
    }
}