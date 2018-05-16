public class bbAddComponentAction : IBoardAction
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();
	
    public virtual int Guid
    {
        get { return ComponentGuid; }
    }

    public IECSComponent Component;
    
    public bool PerformAction(BoardController gameBoardController)
    {
        gameBoardController.RegisterComponent(new EnemiesLogicComponent());
        Component = null;
        return true;
    }
}