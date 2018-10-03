using Newtonsoft.Json;

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
}