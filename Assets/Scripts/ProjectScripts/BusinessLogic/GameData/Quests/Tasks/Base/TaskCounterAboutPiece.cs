using System.Runtime.Serialization;
using Newtonsoft.Json;

/// <summary>
/// Base class for Tasks that require PieceId field.
/// 
/// Warning you should specify one PieceId OR PieceUid in json config!
/// </summary>
public abstract class TaskCounterAboutPiece : TaskCounterEntity, IHavePieceId
{   
    [JsonProperty] public int PieceId { get; protected set; } //[JsonProperty] is for backward compatibility
        
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
}