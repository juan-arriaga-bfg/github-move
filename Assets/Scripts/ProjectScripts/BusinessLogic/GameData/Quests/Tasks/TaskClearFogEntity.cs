using System.Runtime.Serialization;
using Newtonsoft.Json;

[TaskHighlight(typeof(HighlightTaskClearFog))]
public class TaskClearFogEntity : TaskEventCounterEntity
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();
    public override int Guid => ComponentGuid;

    protected override int EventCode => GameEventsCodes.ClearFog;
    
    public string FogId { get; protected set; }
        
#region Serialization

    [JsonProperty] public string PieceUid;
    
    public bool ShouldSerializeFogId()
    {
        return false;
    }    
    
    public bool ShouldSerializePieceUid()
    {
        return false;
    }
    
    [OnDeserialized]
    protected void OnDeserializedTaskCounterAboutPiece(StreamingContext context)
    {
        if (string.IsNullOrEmpty(PieceUid))
        {
            return;
        }
        
        FogId = PieceUid;
    }
    
#endregion
    
    public override void OnBoardEvent(int code, object context)
    {
        if (!IsInProgress())
        {
            return;
        }

        if (code != EventCode)
        {
            return;
        }

        if (string.IsNullOrEmpty(FogId) || (context as FogDef)?.Uid == FogId)
        {
            CurrentValue += 1;
        }
    } 
}