public class MulticellularSpawnPieceBuilder : MulticellularPieceBuilder
{
    public override Piece Build(int pieceType, BoardController context)
    {
        var piece = base.Build(pieceType, context);
        
        PieceDef def;
        var spawnPiece = PieceType.C1.Id;
        var spawnAmount = 1;
        var delay = 0;
        CurrencyPair resourses = null;
		
        if (GameDataService.Current.Pieces.TryGetValue(piece.PieceType, out def))
        {
            spawnPiece = PieceType.Parse(def.SpawnPiece);
            spawnAmount = def.SpawnAmount;
            delay = def.Delay;
            resourses = def.Resources;
        }
        
        piece.RegisterComponent(new TouchReactionComponent()
            .RegisterComponent(new TouchReactionDefinitionSpawnPiece{SpawnPieceType = spawnPiece, SpaunAmaunt = spawnAmount, Resources = resourses})
            .RegisterComponent(new TouchReactonConditionDelay{Delay = delay}));
        
        return piece;
    }
}