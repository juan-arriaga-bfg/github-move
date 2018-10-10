using System.Runtime.Serialization;
using Newtonsoft.Json;

/// <summary>
/// Condition to limit count of Quests running at the same time.
/// </summary>
[JsonObject(MemberSerialization.OptIn)]
public sealed class QuestStartConditionMaxActiveQuestsCountComponent : QuestStartConditionComponent
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();
    public override int Guid => ComponentGuid;

    [JsonProperty] public int Count { get; protected set; }
    
    public override bool Check()
    {
        var quests = GameDataService.Current.QuestsManager.ActiveQuests;
        return quests != null && quests.Count <= Count;
    }
    
    [OnDeserialized]
    internal void OnDeserialized(StreamingContext context)
    {
        Count = int.Parse(Value);
    }
}