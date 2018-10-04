using Newtonsoft.Json;

[JsonObject(MemberSerialization.OptIn)]
public sealed class QuestStartConditionPieceUnlockedComponent : QuestStartConditionComponent
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();
    public override int Guid => ComponentGuid;
    
    [JsonProperty] public int PieceId { get; protected set; }

    public override bool Check()
    {
        return GameDataService.Current.CodexManager.IsPieceUnlocked(PieceId);
    }
}