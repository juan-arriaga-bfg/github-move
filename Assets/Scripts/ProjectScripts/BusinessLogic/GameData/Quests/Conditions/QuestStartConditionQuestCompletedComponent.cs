using System.Runtime.Serialization;
using Newtonsoft.Json;

/// <summary>
/// Check that quest with QuestId have been completed by a user.
/// </summary>
[JsonObject(MemberSerialization.OptIn)]
public sealed class QuestStartConditionQuestCompletedComponent : QuestStartConditionComponent
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();
    public override int Guid => ComponentGuid;
    
    [JsonProperty] public string QuestId { get; protected set; }

    public override bool Check()
    {
        return GameDataService.Current.QuestsManager.CompletedQuests.Contains(QuestId);
    }
    
    [OnDeserialized]
    internal void OnDeserialized(StreamingContext context)
    {
        QuestId = Value;
    }
}