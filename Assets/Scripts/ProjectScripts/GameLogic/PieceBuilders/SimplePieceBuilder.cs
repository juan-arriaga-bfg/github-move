public class SimplePieceBuilder : GenericPieceBuilder 
{
    public override Piece Build(int pieceType, BoardController context)
    {
        var piece = base.Build(pieceType, context);
        
        piece.RegisterComponent(new DraggablePieceComponent());
        
        var def = GameDataService.Current.PiecesManager.GetPieceDef(pieceType);

        if (def == null) return piece;

        if (def.Reproduction != null)
        {
            var typeDef = PieceType.GetDefById(pieceType);

            typeDef.Filter.Add(PieceTypeFilter.Reproduction);
            
            var observer = new ReproductionPieceComponent {Child = def.Reproduction};
        
            piece.RegisterComponent(observer);
            AddObserver(piece, observer);
        }

        if (def.SpawnResources != null)
        {
            var typeDef = PieceType.GetDefById(pieceType);

            typeDef.Filter.Add(PieceTypeFilter.Resource);
            
            piece.RegisterComponent(new ResourceStorageComponent{Resources = def.SpawnResources});
		
            piece.RegisterComponent(new TouchReactionComponent()
                .RegisterComponent(new TouchReactionDefinitionCollectResource())
                .RegisterComponent(new TouchReactionConditionComponent()));
        }
        
        return piece;
    }
}