public class CastlePieceBuilder : MulticellularSpawnPieceBuilder
{
    public override Piece Build(int pieceType, BoardController context)
    {
        var piece = base.Build(pieceType, context);
        
        var observer = piece.GetComponent<PieceBoardObserversComponent>(PieceBoardObserversComponent.ComponentGuid);

        if (observer != null)
        {
            observer.RegisterObserver(new HeroObserver());
        }
        
        return piece;
    }
}