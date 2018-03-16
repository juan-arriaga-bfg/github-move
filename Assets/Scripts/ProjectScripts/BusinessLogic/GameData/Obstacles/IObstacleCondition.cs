public interface IObstacleCondition
{
    bool IsInitialized { get; set; }
    void Init();
    bool Check(ObstaclesLogicComponent context);
}