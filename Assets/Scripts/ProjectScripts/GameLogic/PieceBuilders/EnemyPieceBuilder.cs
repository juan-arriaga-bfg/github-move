public class EnemyPieceBuilder : GenericPieceBuilder
{
    public override Piece Build(int pieceType, BoardController context)
    {
        var piece = base.Build(pieceType, context);

        CreateViewComponent(piece);
        AddObserver(piece, new AreaLockCrossComponent());

        piece.RegisterComponent(new TouchReactionComponent()
            .RegisterComponent(new TouchReactionDefinitionOpenEnemyBubble())
            .RegisterComponent(new TouchReactionConditionComponent()));
        
        return piece;
    }
}