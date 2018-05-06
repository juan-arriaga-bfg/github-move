public class CastlePieceBuilder : MulticellularPieceBuilder
{
    public override Piece Build(int pieceType, BoardController context)
    {
        var piece = base.Build(pieceType, context);
        var def = GameDataService.Current.PiecesManager.GetPieceDefOrDefault(pieceType);
        
        CreateViewComponent(piece);
        
        piece.RegisterComponent(new TimerComponent
        {
            Delay = def.Delay
        });
        
        var storage = new StorageComponent
        {
            SpawnPiece = def.SpawnPieceType,
            Capacity = def.SpawnCapacity,
            Filling = def.IsFilledInStart ? def.SpawnCapacity : 0,
            Amount =  def.SpawnAmount
        };
        
        piece.RegisterComponent(storage);
        AddObserver(piece, storage);
        
        piece.RegisterComponent(new TouchReactionComponent()
            .RegisterComponent(new TouchReactionDefinitionMenu()
                .RegisterDefinition(new TouchReactionDefinitionUpgradeCastle{Icon = "arrow_light"})
                .RegisterDefinition(new TouchReactionDefinitionSpawnInStorage{Icon = "Chest"})
                .RegisterDefinition(new TouchReactionDefinitionOpenHeroesWindow{Icon = "face_Robin"}))
            .RegisterComponent(new TouchReactionConditionComponent()));
        
       /*
        AddView(piece, ViewType.LevelLabel);
        AddView(piece, ViewType.Menu);
        AddView(piece, ViewType.BoardTimer);
        AddView(piece, ViewType.CastleUpgrade);*/
        
        return piece;
    }
}