using System.Runtime.Serialization;
using Newtonsoft.Json;
using UnityEngine;

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
        if (!string.IsNullOrEmpty(PieceUid))
        {
            PieceId = PieceType.Parse(PieceUid);
        }

        var validationError = ValidateDeserializedData();
        if (!string.IsNullOrEmpty(validationError))
        {
            Debug.LogError($"[TaskCounterAboutPiece] => [OnDeserialized]: Task with id '{Id}': Validation failed: {validationError}");
        }
    }

    /// <summary>
    /// Override to made any checks immediately after instantiation from json
    /// </summary>
    /// <returns>
    /// Null if all ok and message if fail
    /// </returns>
    protected virtual string ValidateDeserializedData()
    {
        return null;
    }
    
#endregion
}