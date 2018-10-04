using System.Runtime.Serialization;
using Newtonsoft.Json;

/// <summary>
/// Warning you should specify one PieceId OR PieceUid in json config!
/// </summary>
public abstract class TaskCounterAboutPiece : TaskCounterEntity, IHavePieceId
{
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
    
#region Serialization

    public bool ShouldSerializePieceId()
    {
        return false;
    }    
    
    public bool ShouldSerializePieceUid()
    {
        return false;
    }
    
#endregion
}