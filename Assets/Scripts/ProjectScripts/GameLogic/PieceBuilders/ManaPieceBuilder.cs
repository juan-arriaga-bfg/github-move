public class ManaPieceBuilder : GenericPieceBuilder 
{
    public override Piece Build(int pieceType, BoardController context)
    {
        var piece = base.Build(pieceType, context);

        piece.RegisterComponent(new ManaDraggablePieceComponent());
        
        var def = GameDataService.Current.PiecesManager.GetPieceDef(pieceType);

        if (def == null) return piece;
        
        piece.RegisterComponent(new TutorialLockerComponent {Step = TutorialBuilder.FogStepIndex});
        piece.RegisterComponent(new ResourceStorageComponent {Resources = def.SpawnResources});

        piece.RegisterComponent(new TouchReactionComponent()
            .RegisterComponent(new TouchReactionDefinitionCollectMana())
            .RegisterComponent(new TouchReactionConditionMana()));
        
        AddObserver(piece, new ManaPieceBoardObserver());
        
        return piece;
    }
}