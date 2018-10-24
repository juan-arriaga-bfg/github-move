using System.Runtime.Serialization;
using Newtonsoft.Json;

[TaskHighlight(typeof(HighlightTaskUseMine))]
[TaskHighlight(typeof(HighlightTaskNextFog))]
public class TaskUseMineEntity : TaskEventCounterEntity
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();
    public override int Guid => ComponentGuid;

    protected override int EventCode => GameEventsCodes.MineUsed;

    public int PieceId { get; protected set; } = -1;
        
#region Serialization

    [JsonProperty] public string PieceUid;
    
    public bool ShouldSerializePieceId()
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
        
        PieceId = PieceType.Parse(PieceUid);
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

        if (PieceId < 0 || (context as MineLifeComponent)?.MinePieceType == PieceId)
        {
            CurrentValue += 1;
        }
    } 
}