using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;

public interface IEventGameSaveComponent
{
    EventGameSaveComponent EventGameSave { get; }
}

public partial class UserProfile : IEventGameSaveComponent
{
    protected EventGameSaveComponent EventGameSaveComponent;

    [JsonIgnore]
    public EventGameSaveComponent EventGameSave => EventGameSaveComponent ?? (EventGameSaveComponent = GetComponent<EventGameSaveComponent>(EventGameSaveComponent.ComponentGuid));
}

[JsonObject(MemberSerialization.OptIn)]
public class EventGameSaveComponent : ECSEntity, IECSSerializeable
{
    public static int ComponentGuid = ECSManager.GetNextGuid();
    public override int Guid => ComponentGuid;
    
    private List<EventGameSaveItem> steps;
    
    [JsonProperty]
    public List<EventGameSaveItem> Steps
    {
        get { return steps; }
        set { steps = value; }
    }
    
    [OnSerializing]
    internal void OnSerialization(StreamingContext context)
    {
        if (GameDataService.Current?.EventGameManager?.Defs == null) return;
        
        var data = GameDataService.Current.EventGameManager.Defs;
        
        steps = new List<EventGameSaveItem>();

        foreach (var pair in data)
        {
            foreach (var def in pair.Value.Steps)
            {
                steps.Add(new EventGameSaveItem
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