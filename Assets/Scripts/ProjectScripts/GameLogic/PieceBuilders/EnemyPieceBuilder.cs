public class EnemyPieceBuilder : GenericPieceBuilder
{
    public override Piece Build(int pieceType, BoardController context)
    {
        var piece = base.Build(pieceType, context);

        CreateViewComponent(piece);

        var areaLockComponent = new AreaLockCrossComponent();
        
        AddObserver(piece, areaLockComponent);
        piece.RegisterComponent(areaLockComponent);

        piece.RegisterComponent(new TouchReactionComponent()
            .RegisterComponent(new TouchReactionDefinitionOpenEnemyBubble())
            .RegisterComponent(new TouchReactionConditionComponent()));
        
        return piece;
    }
}