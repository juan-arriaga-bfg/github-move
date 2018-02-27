public class MulticellularSpawnPieceBuilder : MulticellularPieceBuilder
{
    public int SpawnPieceType;
    public int Delay;
    
    public override Piece Build(int pieceType, BoardController context)
    {
        var piece = base.Build(pieceType, context);
        
        piece.RegisterComponent(new TouchReactionComponent()
            .RegisterComponent(new TouchReactionDefinitionSpawnPieceComponent{SpawnPieceType = SpawnPieceType})
            .RegisterComponent(new TouchReactonConditionDelayComponent{Delay = Delay}));
        
        return piece;
    }
}