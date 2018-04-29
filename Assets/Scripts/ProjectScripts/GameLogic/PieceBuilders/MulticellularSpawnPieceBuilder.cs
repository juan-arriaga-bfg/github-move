public class MulticellularSpawnPieceBuilder : MulticellularPieceBuilder
{
    public override Piece Build(int pieceType, BoardController context)
    {
        var piece = base.Build(pieceType, context);
        var def = GameDataService.Current.PiecesManager.GetPieceDefOrDefault(pieceType);
        
        piece.RegisterComponent(new TimerComponent
        {
            Delay = def.Delay
        });
        
        piece.RegisterComponent(new StorageComponent
        {
            SpawnPiece = def.SpawnPieceType,
            Capacity = def.SpawnCapacity,
            Filling = def.IsFilledInStart ? def.SpawnCapacity : 0,
            Amount = def.SpawnAmount
        });
        
        piece.RegisterComponent(new TouchReactionComponent()
            .RegisterComponent(new TouchReactionDefinitionMenu()
                .RegisterDefinition(new TouchReactionDefinitionUpgrade {Icon = "arrow_light"})
                .RegisterDefinition(new TouchReactionDefinitionSpawnInStorage{Icon = PieceType.Parse(def.SpawnPieceType)}))
            .RegisterComponent(new TouchReactionConditionComponent()));
        
        AddView(piece, ViewType.StorageState);
        AddView(piece, ViewType.LevelLabel);
        AddView(piece, ViewType.Menu);
        
        return piece;
    }
}