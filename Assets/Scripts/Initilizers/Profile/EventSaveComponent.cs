using System.Collections.Generic;
using System.Runtime.Serialization;
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

[JsonObject(MemberSerialization.OptIn)]
public class EventSaveComponent : ECSEntity, IECSSerializeable
{
    public static int ComponentGuid = ECSManager.GetNextGuid();
    public override int Guid => ComponentGuid;
    
    private List<EventSaveItem> steps;
    
    [JsonProperty]
    public List<EventSaveItem> Steps
    {
        get { return steps; }
        set { steps = value; }
    }
    
    [OnSerializing]
    internal void OnSerialization(StreamingContext context)
    {
        if (GameDataService.Current?.EventManager?.Defs == null) return;
        
        var data = GameDataService.Current.EventManager.Defs;
        
        steps = new List<EventSaveItem>();

        foreach (var pair in data)
        {
            foreach (var def in pair.Value)
            {
                steps.Add(new EventSaveItem
                {
                    Key = pair.Key,
                    Step = def.Step,
                    IsNormalClaimed = def.IsNormalClaimed,
                    IsPremiumClaimed = def.IsPremiumClaimed
                });
            }
        }
    }
    
    [OnDeserialized]
    internal void OnDeserialized(StreamingContext context)
    {
    }
}