public class ChestPieceComponent : IECSComponent, IPieceBoardObserver, ITimerComponent
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();
    
    public int Guid { get { return ComponentGuid; } }

    public Chest Chest;

    private TimerComponent timer;
    private Piece contextPiece;
    
    public void OnRegisterEntity(ECSEntity entity)
    {
        contextPiece = entity as Piece;
        
        timer = new TimerComponent();
        contextPiece.RegisterComponent(timer);
    }
    
    public void OnUnRegisterEntity(ECSEntity entity)
    {
    }

    public void OnAddToBoard(BoardPosition position, Piece context = null)
    {
        if(Chest != null) return;
        
        Chest = GameDataService.Current.ChestsManager.GetFromBoard(position, contextPiece.PieceType);
        timer.Delay = Chest.Def.Time;
    }

    public void OnMovedFromTo(BoardPosition from, BoardPosition to, Piece context = null)
    {
    }

    public void OnRemoveFromBoard(BoardPosition position, Piece context = null)
    {
    }

    public TimerComponent Timer
    {
        get { return timer; }
    }
}