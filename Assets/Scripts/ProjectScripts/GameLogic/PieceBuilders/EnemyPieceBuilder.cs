public class EnemyPieceBuilder : GenericPieceBuilder
{
    public override Piece Build(int pieceType, BoardController context)
    {
        var piece = base.Build(pieceType, context);

        CreateViewComponent(piece);

        var areaLockComponent = new AreaLockComponent();
        AddObserver(piece, areaLockComponent);
        piece.RegisterComponent(areaLockComponent)
             .RegisterComponent(new TouchReactionComponent()
                               .RegisterComponent(new TouchReactionDefinitionOpenEnemyBubble() {ViewId = ViewType.Bubble})
                                // .RegisterComponent(new TouchReactionDefinitionMenu {MainReactionIndex = 0}
                                //                   .RegisterDefinition(new TouchReactionDefinitionOpenBubble {ViewId = ViewType.Bubble})
                                //                   // .RegisterDefinition(new TouchReactionDefinitionObstacleComponent {IsAutoStart = false}))
                                //      )
                               .RegisterComponent(new TouchReactionConditionComponent())
              );

        // .RegisterComponent(new PiecePathfindBoardCondition(context, piece)
        //                   .RegisterComponent(PathfindIgnoreBuilder.Build(piece.PieceType)));

        return piece;
    }
}