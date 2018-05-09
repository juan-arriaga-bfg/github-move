public class ChestPieceComponent : IECSComponent, IPieceBoardObserver, ITimerComponent
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();
    
    public int Guid { get { return ComponentGuid; } }

    public Chest Chest;

    private TimerComponent timer;
    private Piece contextPiece;
    private ViewDefinitionComponent viewDef;
    
    public TimerComponent Timer
    {
        get { return timer; }
    }
    
    public void OnRegisterEntity(ECSEntity entity)
    {
        contextPiece = entity as Piece;
        
        timer = new TimerComponent();
        contextPiece.RegisterComponent(timer);
        viewDef = contextPiece.GetComponent<ViewDefinitionComponent>(ViewDefinitionComponent.ComponentGuid);
    }
    
    public void OnUnRegisterEntity(ECSEntity entity)
    {
    }

    public void OnAddToBoard(BoardPosition position, Piece context = null)
    {
        if(Chest != null) return;
        
        Chest = GameDataService.Current.ChestsManager.GetFromBoard(position, contextPiece.PieceType);
        
        timer.Delay = Chest.Def.Time;
        timer.OnStart += OnStart;
        timer.OnStop += OnComplete;
        timer.OnComplete += OnComplete;

        if (Chest.StartTime != null) timer.Start(Chest.StartTime.Value);
    }

    public void OnMovedFromTo(BoardPosition from, BoardPosition to, Piece context = null)
    {
    }

    public void OnRemoveFromBoard(BoardPosition position, Piece context = null)
    {
        timer.OnStart -= OnStart;
        timer.OnComplete -= OnComplete;
    }

    private void OnStart()
    {
        if(viewDef == null) return;
        
        var view = viewDef.AddView(ViewType.BoardTimer);
        
        view.Change(true);
    }
    
    private void OnComplete()
    {
        if(viewDef == null) return;
        
        var view = viewDef.AddView(ViewType.BoardTimer);
        
        view.Change(false);
        
        if(Chest.State != ChestState.Open) return;
        
        if(GameDataService.Current.ChestsManager.ActiveChest == Chest)
        {
            GameDataService.Current.ChestsManager.ActiveChest = null;
        }
        
        var hint = contextPiece.Context.GetComponent<HintCooldownComponent>(HintCooldownComponent.ComponentGuid);
        
        if(hint == null) return;
        
        hint.Step(HintType.OpenChest);
    }
}