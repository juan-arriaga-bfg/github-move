using System;

public class TutorialLockerComponent : IECSComponent
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();
    public virtual int Guid => ComponentGuid;

    public int Step;
    
    private Action onTouch;
    private Action onComplete;

    private Piece context;
    
    public void OnRegisterEntity(ECSEntity entity)
    {
        context = entity as Piece;
        
        if(GameDataService.Current.TutorialDataManager.IsCompeted(Step) == false) return;

        context.UnRegisterComponent(this);
    }

    public void OnUnRegisterEntity(ECSEntity entity)
    {
        context.TutorialLocker = null;
    }
    
    public void SetTouchAction(Action value)
    {
        onTouch += value;
    }
    
    public void SetCompleteAction(Action value)
    {
        onComplete += value;
    }
    
    public void Touch()
    {
        onTouch?.Invoke();
    }
    
    public void Complete()
    {
        onComplete?.Invoke();
        
        onComplete = null;
        onTouch = null;
        
        context.UnRegisterComponent(this);
    }
}
