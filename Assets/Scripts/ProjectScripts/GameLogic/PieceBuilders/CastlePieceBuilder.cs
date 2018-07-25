using UnityEngine;

public class CastlePieceBuilder : MulticellularPieceBuilder
{
    public override Piece Build(int pieceType, BoardController context)
    {
        var piece = base.Build(pieceType, context);
        var def = GameDataService.Current.PiecesManager.GetPieceDefOrDefault(pieceType);
        
        AddView(piece, ViewType.LevelLabel);

        piece.RegisterComponent(new DraggablePieceComponent());
        
        piece.RegisterComponent(new TimerComponent
        {
            Delay = def.Delay
        });
        
        var storage = new StorageComponent
        {
            SpawnPiece = def.SpawnPieceType,
            Capacity = def.SpawnCapacity,
            Filling = def.IsFilledInStart ? def.SpawnCapacity : 0,
            Amount =  def.SpawnAmount,
            IsTimerShow = true,
            TimerOffset = new Vector2(-0.9f, 2.5f)
        };
        
        var upgrade = new CastleUpgradeComponent();
        
        piece.RegisterComponent(storage);
        AddObserver(piece, storage);
        
        piece.RegisterComponent(upgrade);
        AddObserver(piece, upgrade);
        
        piece.RegisterComponent(new TouchReactionComponent()
            .RegisterComponent(new TouchReactionDefinitionMenu{MainReactionIndex = 0}
                .RegisterDefinition(new TouchReactionDefinitionOpenWindow{WindowType = UIWindowType.CastleWindow})
                .RegisterDefinition(new TouchReactionDefinitionUpgrade()))
            .RegisterComponent(new TouchReactionConditionComponent()));
        
        return piece;
    }
}