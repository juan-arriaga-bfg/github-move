public class MulticellularSpawnPieceBuilder : MulticellularPieceBuilder
{
    public override Piece Build(int pieceType, BoardController context)
    {
        var piece = base.Build(pieceType, context);
        var def = GameDataService.Current.PiecesManager.GetPieceDefOrDefault(pieceType);

        AddView(piece, ViewType.LevelLabel);
        
        piece.RegisterComponent(new TimerComponent
        {
            Delay = def.Delay
        });
        
        var storage = new StorageComponent
        {
            SpawnPiece = def.SpawnPieceType,
            Capacity = def.SpawnCapacity,
            Filling = def.IsFilledInStart ? def.SpawnCapacity : 0,
            Amount = def.SpawnAmount
        };
        
        piece.RegisterComponent(storage);
        AddObserver(piece, storage);
        
        piece.RegisterComponent(new TouchReactionComponent()
            .RegisterComponent(new TouchReactionDefinitionMenu()
                .RegisterDefinition(new TouchReactionDefinitionUpgrade {Icon = "arrow_light"})
                .RegisterDefinition(new TouchReactionDefinitionSpawnInStorage{Icon = PieceType.Parse(def.SpawnPieceType)}))
            .RegisterComponent(new TouchReactionConditionComponent()));
        
        /* AddView(piece, ViewType.Menu);*/
        
        return piece;
    }
}