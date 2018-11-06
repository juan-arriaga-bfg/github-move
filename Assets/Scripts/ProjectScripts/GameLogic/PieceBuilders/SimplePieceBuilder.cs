using System.Collections.Generic;
using UnityEngine;

public class SimplePieceBuilder : GenericPieceBuilder 
{
    public override Piece Build(int pieceType, BoardController context)
    {
        var piece = base.Build(pieceType, context);

        piece.RegisterComponent(pieceType == PieceType.Boost_WR.Id ? new WorkerDraggablePieceComponent() : new DraggablePieceComponent());

        var def = GameDataService.Current.PiecesManager.GetPieceDef(pieceType);
        
        if (def == null) return piece;

        if (def.ReproductionDef?.Reproduction != null)
        {
            var observer = new ReproductionPieceComponent {Child = def.ReproductionDef.Reproduction};
        
            piece.RegisterComponent(observer);
            AddObserver(piece, observer);
        }

        piece.RegisterComponent(
            new PiecePathfindBoardCondition(piece.Context, piece)
                .RegisterComponent(PathfindIgnoreBuilder.Build(piece.PieceType)
        ));
        
        if (def.SpawnResources != null || PieceType.GetDefById(pieceType).Filter.Has(PieceTypeFilter.Resource))
        {
            piece.RegisterComponent(new ResourceStorageComponent {Resources = def.SpawnResources});

            piece.RegisterComponent(new TouchReactionComponent()
                .RegisterComponent(new TouchReactionDefinitionCollectResource())
                .RegisterComponent(new TouchReactionConditionComponent()));
            
        }

        AddPathfindLockObserver(piece, true);
        
        return piece;
    }
}