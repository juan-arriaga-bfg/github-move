public class ChestPieceBuilder : SimplePieceBuilder
{
    public override Piece Build(int pieceType, BoardController context)
    {
        var piece = base.Build(pieceType, context);
        
        piece.RegisterComponent(new TouchReactionComponent()
            .RegisterComponent(new TouchReactionDefinitionOpenChestRewardWindow())
            .RegisterComponent(new TouchReactionConditionComponent()));

        return piece;
    }
}