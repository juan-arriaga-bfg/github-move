using System.Runtime.Serialization;
using Newtonsoft.Json;

/// <summary>
/// Check that fog with id is cleared by a user.
/// </summary>
[JsonObject(MemberSerialization.OptIn)]
public sealed class QuestStartConditionClearFogComponent : QuestStartConditionComponent
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();
    public override int Guid => ComponentGuid;
    
    [JsonProperty] public string FogUid { get; protected set; }
    
    [OnDeserialized]
    internal void OnDeserialized(StreamingContext context)
    {
        FogUid = Value;
    }

    public override bool Check()
    {
        return GameDataService.Current.FogsManager.IsFogCleared(FogUid);
    }
}