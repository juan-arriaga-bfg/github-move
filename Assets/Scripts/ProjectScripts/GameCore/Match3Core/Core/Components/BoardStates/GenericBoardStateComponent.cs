
public class GenericBoardStateComponent : IECSComponent
{
    public static int ComponentGuid = ECSManager.GetNextGuid();

    public virtual int Guid { get { return ComponentGuid; } }
    
    public void OnRegisterEntity(ECSEntity entity) { }

    public void OnUnRegisterEntity(ECSEntity entity) { }

    protected int stateId;

    public virtual int StateId 
    { 
        get { return stateId; } 
        set { stateId = value; } 
    }
}
