using System.Runtime.Serialization;
using Newtonsoft.Json;

[TaskHighlight(typeof(HighlightTaskCompleteOrderCharacter))]
[TaskHighlight(typeof(HighlightTaskPointToOrdersButton))]
public class TaskCompleteOrderEntity : TaskEventCounterEntity
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();
    public override int Guid => ComponentGuid;

    protected override int EventCode => GameEventsCodes.OrderCompleted;
}