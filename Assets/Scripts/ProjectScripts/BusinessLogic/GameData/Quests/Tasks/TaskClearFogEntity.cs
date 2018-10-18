[TaskHighlight(typeof(HighlightTaskClearFog))]
public class TaskClearFogEntity : TaskEventCounterEntity
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();
    public override int Guid => ComponentGuid;

    protected override int EventCode => GameEventsCodes.ClearFog;
}