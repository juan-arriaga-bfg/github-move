using System.Collections.Generic;

public class ReproductionPieceComponent : IECSComponent, IPieceBoardObserver
{
    public static int ComponentGuid = ECSManager.GetNextGuid();
    public int Guid
    {
        get { return ComponentGuid; }
    }
    
    public CurrencyPair Child { get; set; }

    private Piece contextPiece;
    private EmptyCellsFinderComponent emptyFinder;
    
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
        contextPiece.Context.ReproductionLogic.Add(this);
    }

    public void OnMovedFromTo(BoardPosition @from, BoardPosition to, Piece context = null)
    {
    }

    public void OnRemoveFromBoard(BoardPosition position, Piece context = null)
    {
        contextPiece.Context.ReproductionLogic.Remove(this);
    }
    
    public void Reproduction()
    {
        var field = new List<BoardPosition>();
        
        if (emptyFinder.FindRandomNearWithPointInCenter(contextPiece.CachedPosition, field, Child.Amount) == false)
        {
            return;
        }
        
        contextPiece.Context.ActionExecutor.AddAction(new ReproductionPieceAction()
        {
            From = contextPiece.CachedPosition,
            Piece = PieceType.Parse(Child.Currency),
            Positions = field
        });
    }

    public bool CheckFreePosition()
    {
        return emptyFinder.CheckWithPointInCenter(contextPiece.CachedPosition);
    }
}