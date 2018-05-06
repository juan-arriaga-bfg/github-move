public class ChestPieceBuilder : SimplePieceBuilder
{
    public override Piece Build(int pieceType, BoardController context)
    {
        var piece = base.Build(pieceType, context);
        
        CreateViewComponent(piece);
        
        var observer = new ChestPieceComponent();
        
        piece.RegisterComponent(observer);
        AddObserver(piece, observer);
        
        piece.RegisterComponent(new TouchReactionComponent()
            .RegisterComponent(new TouchReactionDefinitionOpenChestRewardWindow())
            .RegisterComponent(new TouchReactionConditionComponent()));

        return piece;
    }

    protected override void AddMatchableComponent(Piece piece)
    {
        piece.RegisterComponent(new MatchableChestComponent());
    }
}