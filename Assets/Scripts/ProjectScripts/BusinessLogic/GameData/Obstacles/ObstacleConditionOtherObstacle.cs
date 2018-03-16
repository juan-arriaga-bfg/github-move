public class ObstacleConditionOtherObstacle : IObstacleCondition
{
    public int OtherUid { get; set; }
    public ObstacleState State { get; set; }
    
    public bool IsInitialized { get; set; }
    
    public void Init()
    {
        IsInitialized = true;
    }

    public bool Check(ObstaclesLogicComponent context)
    {
        if (IsInitialized == false) return false;

        var obstacle = context.GetObstacle(OtherUid);
        
        return obstacle == null || obstacle.State == State;
    }
}