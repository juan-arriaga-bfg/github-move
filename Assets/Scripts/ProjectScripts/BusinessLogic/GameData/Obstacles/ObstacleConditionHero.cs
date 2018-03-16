public class ObstacleConditionHero : IObstacleCondition
{
    public string Hero { get; set; }
    
    public bool IsInitialized { get; set; }

    public void Init()
    {
        IsInitialized = true;
    }

    public bool Check(ObstaclesLogicComponent context)
    {
        if (IsInitialized == false) return false;
        return true;
    }
}