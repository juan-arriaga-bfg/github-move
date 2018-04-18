public class ChestPieceComponent : IECSComponent, IPieceBoardObserver
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();
    
    public int Guid { get { return ComponentGuid; } }

    public Chest Chest;
    private Piece contextPiece;
    
    public void OnRegisterEntity(ECSEntity entity)
    {
        contextPiece = entity as Piece;
    }

    public void OnUnRegisterEntity(ECSEntity entity)
    {
    }

    public void OnAddToBoard(BoardPosition position, Piece context = null)
    {
        Chest = GameDataService.Current.ChestsManager.GetFromBoard(position);

        if (Chest != null) return;
        
        Chest = GameDataService.Current.ChestsManager.GetChest(contextPiece.PieceType);
        GameDataService.Current.ChestsManager.AddToBoard(position, Chest.ChestType);
    }

    public void OnMovedFromTo(BoardPosition from, BoardPosition to, Piece context = null)
    {
        if(Chest == null) return;
        
        GameDataService.Current.ChestsManager.MovedFromToBoard(from, to);
    }

    public void OnRemoveFromBoard(BoardPosition position, Piece context = null)
    {
        if(Chest == null) return;
        
        GameDataService.Current.ChestsManager.RemoveFromBoard(position);
    }
}