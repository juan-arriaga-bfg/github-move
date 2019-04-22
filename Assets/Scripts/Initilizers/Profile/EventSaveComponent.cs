using System.Collections.Generic;
using Newtonsoft.Json;

public interface IEventSaveComponent
{
    EventSaveComponent EventSave { get; }
}

public partial class UserProfile : IEventSaveComponent
{
    protected EventSaveComponent eventSaveComponent;

    [JsonIgnore]
    public EventSaveComponent EventSave => eventSaveComponent ?? (eventSaveComponent = GetComponent<EventSaveComponent>(EventSaveComponent.ComponentGuid));
}

public class EventSaveComponent : ECSEntity, IECSSerializeable
{
    public static int ComponentGuid = ECSManager.GetNextGuid();
    public override int Guid => ComponentGuid;
    
    private List<EventStepDef> steps;
    
    [JsonProperty]
    public List<EventStepDef> Steps
    {
        get { return steps; }
        set { steps = value; }
    }
}