using Newtonsoft.Json;

[JsonObject(MemberSerialization.OptIn)]
public sealed class QuestStartConditionLevelComponent : QuestStartConditionComponent
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();
    public override int Guid => ComponentGuid;
    
    [JsonProperty] public int UserLevel { get; protected set; }

    public override bool Check()
    {
        return UserLevel <= ProfileService.Current.GetStorageItem(Currency.Level.Name).Amount;
    }
}