public class ReproductionPieceComponent : IECSComponent, IPieceBoardObserver
{
    public static int ComponentGuid = ECSManager.GetNextGuid();
    public int Guid => ComponentGuid;

    public CurrencyPair Child { get; set; }

    private Piece contextPiece;
    private EmptyCellsFinderComponent emptyFinder;
    private ReproductionLifeComponent life;
    
    public void OnRegisterEntity(ECSEntity entity)
    {
        contextPiece = entity as Piece;
        emptyFinder = contextPiece.Context.BoardLogic.EmptyCellsFinder;
    }

    public void OnUnRegisterEntity(ECSEntity entity)
    {
    }
    
    public void OnAddToBoard(BoardPosition position, Piece context = null)
    {
        if (life == null) life = contextPiece.GetComponent<ReproductionLifeComponent>(ReproductionLifeComponent.ComponentGuid);
    }

    public void OnMovedFromToStart(BoardPosition @from, BoardPosition to, Piece context = null)
    {
    }

    public void OnMovedFromToFinish(BoardPosition @from, BoardPosition to, Piece context = null)
    {
    }

    public void OnRemoveFromBoard(BoardPosition position, Piece context = null)
    {
    }
}