using System.Runtime.Serialization;
using Newtonsoft.Json;

/// <summary>
/// Check that player already saw a piece with PieceId. Happens when piece appears on the Board. Example:
/// 
/// [TaskHighlight(typeof(HighlightTaskClearFog))]
/// public class TaskClearFogEntity : TaskEventCounterEntity
/// {
///     [....]
/// }
/// </summary>
/// 
[JsonObject(MemberSerialization.OptIn)]
public sealed class QuestStartConditionPieceUnlockedComponent : QuestStartConditionComponent
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();
    public override int Guid => ComponentGuid;
    
    [JsonProperty] private string PieceUid;
    
    [JsonProperty] public int PieceId { get; protected set; }
    
    [OnDeserialized]
    internal void OnDeserialized(StreamingContext context)
    {
        if (string.IsNullOrEmpty(PieceUid))
        {
            return;
        }
        
        PieceId = PieceType.Parse(PieceUid);
    }

    public override bool Check()
    {
        return GameDataService.Current.CodexManager.IsPieceUnlocked(PieceId);
    }
}