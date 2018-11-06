[TaskHighlight(typeof(HighlightTaskNotImplemented))]
public class TaskKillObstacleEntity : TaskEventCounterEntity
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();
    public override int Guid => ComponentGuid;

    protected override int EventCode => GameEventsCodes.ObstacleKilled;
}