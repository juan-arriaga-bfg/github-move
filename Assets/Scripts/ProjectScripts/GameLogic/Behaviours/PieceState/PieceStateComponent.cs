public enum PieceLifeState
{
    Waiting,
    Warning,
    InProgress,
    Complete
}

public class PieceStateComponent : IECSComponent
{
    public static int ComponentGuid = ECSManager.GetNextGuid();
    public int Guid => ComponentGuid;
    
    private Piece context;

    private PieceLifeState state;
    public PieceLifeState State
    {
        get { return state; }
        set
        {
            state = value;

            if (state == PieceLifeState.Complete)
            {
                context.Matchable?.Locker.Unlock(this);
                return;
            }
            
            context.Matchable?.Locker.Lock(this, false);
        }
    }

    public void OnRegisterEntity(ECSEntity entity)
    {
        context = entity as Piece;
        State = PieceLifeState.Complete;
    }

    public void OnUnRegisterEntity(ECSEntity entity)
    {
    }

    private bool InitInitInSave(BoardPosition position)
    {
        return false;
    }
}