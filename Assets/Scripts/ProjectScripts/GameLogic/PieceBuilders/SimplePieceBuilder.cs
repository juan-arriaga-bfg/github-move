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
        
        if (def.SpawnResources != null || PieceType.GetDefById(pieceType).Filter.Has(PieceTypeFilter.Resource))
        {
            piece.RegisterComponent(new ResourceStorageComponent{Resources = def.SpawnResources});
		
            piece.RegisterComponent(new TouchReactionComponent()
                 .RegisterComponent(new TouchReactionDefinitionCollectResource())
                 .RegisterComponent(new TouchReactionConditionComponent()));
        }
        
        var pathfindLockObserver = new PathfindLockObserver() {AutoLock = false}; 
        
        AddObserver(piece, pathfindLockObserver);
        piece.RegisterComponent(pathfindLockObserver);
        
        return piece;
    }
}