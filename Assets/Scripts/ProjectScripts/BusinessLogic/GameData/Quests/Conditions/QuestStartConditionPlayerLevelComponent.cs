using Newtonsoft.Json;

/// <summary>
/// Check actual level of a player
/// </summary>
[JsonObject(MemberSerialization.OptIn)]
public sealed class QuestStartConditionPlayerLevelComponent : QuestStartConditionComponent
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();
    public override int Guid => ComponentGuid;
    
    [JsonProperty] public int Level { get; protected set; }

    public override bool Check()
    {
        return Level <= ProfileService.Current.GetStorageItem(Currency.Level.Name).Amount;
    }
}