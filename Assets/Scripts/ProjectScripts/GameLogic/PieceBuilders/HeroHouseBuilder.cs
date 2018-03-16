public class HeroHouseBuilder : MulticellularPieceBuilder
{
    public override Piece Build(int pieceType, BoardController context)
    {
        var piece = base.Build(pieceType, context);
        
        piece.RegisterComponent(new TouchReactionComponent()
            .RegisterComponent(new TouchReactionDefinitionOpenCharacterWindow())
            .RegisterComponent(new TouchReactionConditionComponent()));

        var observer = piece.GetComponent<PieceBoardObserversComponent>(PieceBoardObserversComponent.ComponentGuid);

        if (observer != null)
        {
            observer.RegisterObserver(new HeroObserver());
        }
        
        return piece;
    }
}