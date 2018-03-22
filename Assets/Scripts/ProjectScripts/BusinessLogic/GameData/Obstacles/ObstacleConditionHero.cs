public class ObstacleConditionHero : IObstacleCondition
{
    public HeroAbility HeroAbility { get; set; }
    
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