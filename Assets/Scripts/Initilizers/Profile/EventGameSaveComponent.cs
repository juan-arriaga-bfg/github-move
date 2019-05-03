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
        if (BoardService.Current?.FirstBoard?.BoardLogic.EventGamesLogic == null) return;
        
        var data = BoardService.Current?.FirstBoard?.BoardLogic.EventGamesLogic.EventGames;
        
        eventGames = new List<EventGameSaveItem>();

        foreach (var pair in data.Values)
        {
            foreach (var def in pair.Values)
            {
                var steps = new List<KeyValuePair<bool, bool>>();

                if (def.State == EventGameState.Default) continue;

                foreach (var step in def.Steps)
                {
                    steps.Add(new KeyValuePair<bool, bool>(step.IsNormalClaimed, step.IsPremiumClaimed));
                }
                
                eventGames.Add(new EventGameSaveItem
                {
                    Key = def.EventType,
                    State = def.State,
                    StartTime = def.StartTimeLong,
                    EndTime = def.EndTime.ConvertToUnixTime(),
                    Intro = def.IntroDuration,
                    Steps = steps
                });
            }
        }
    }
}