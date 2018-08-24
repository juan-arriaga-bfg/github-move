using System;

public enum PieceLifeState
{
    Waiting,
    Warning,
    InProgress,
    Complete
}

public class PieceStateComponent : ECSEntity
{
    public static int ComponentGuid = ECSManager.GetNextGuid();
    public override int Guid => ComponentGuid;
    
    public TimerComponent Timer;
    public Action OnChange;
    
    private Piece context;
    
    private BoardTimerView view;
    
    private PieceLifeState state;
    public PieceLifeState State
    {
        get { return state; }
        set
        {
            state = value;
            
            OnChange?.Invoke();
            
            if (state == PieceLifeState.InProgress) OnStart();
            
            if (state == PieceLifeState.Complete)
            {
                context.Matchable?.Locker.Unlock(this);
                return;
            }
            
            context.Matchable?.Locker.Lock(this, false);
        }
    }

    public override void OnRegisterEntity(ECSEntity entity)
    {
        context = entity as Piece;
        State = PieceLifeState.Complete;
        
        Timer = new TimerComponent{Delay = 10};
        Timer.OnComplete += OnComplete;
        RegisterComponent(Timer);
    }

    private bool InitInitInSave(BoardPosition position)
    {
        return false;
    }

    private void OnStart()
    {
        if (view == null)
        {
            var viewDef = context.GetComponent<ViewDefinitionComponent>(ViewDefinitionComponent.ComponentGuid);
            view = viewDef.AddView(ViewType.BoardTimer) as BoardTimerView;
        }
        
        view.SetTimer(Timer);
        Timer.Start();
        view.Change(true);
    }
    
    private void OnComplete()
    {
        view.Change(false);
        State = PieceLifeState.Complete;
    }
}