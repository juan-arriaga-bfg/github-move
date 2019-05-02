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
    
    private List<EventGameSaveItem> eventGames;
    
    [JsonProperty]
    public List<EventGameSaveItem> EventGames
    {
        get => eventGames;
        set => eventGames = value;
    }
    
    [OnSerializing]
    internal void OnSerialization(StreamingContext context)
    {
        if (GameDataService.Current?.EventGameManager?.Defs == null) return;
        
        var data = GameDataService.Current.EventGameManager.Defs;
        
        eventGames = new List<EventGameSaveItem>();

        foreach (var def in data.Values)
        {
            var steps = new List<KeyValuePair<bool, bool>>();

            foreach (var step in def.Steps)
            {
                steps.Add(new KeyValuePair<bool, bool>(step.IsNormalClaimed, step.IsPremiumClaimed));
            }
            
            eventGames.Add(new EventGameSaveItem
            {
                Key = def.EventType,
                State = def.State,
                Steps = steps
            });
        }
    }
}