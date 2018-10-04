using Newtonsoft.Json;

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
}