using System.Runtime.Serialization;
using Newtonsoft.Json;
using UnityEngine;

public class TaskSuperMatchEntity : TaskCounterAboutPiece, IBoardEventListener
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();
    public override int Guid => ComponentGuid;

    [JsonProperty] public int CountToMatch { get; protected set; } // [JsonProperty] is for a backward compatibility
    
    [JsonProperty] public int Value1 { get; protected set; }
    
#region Serialization

    public bool ShouldSerializeCountToMatch()
    {
        return false;
    }   
    
    public bool ShouldSerializeValue1()
    {
        return false;
    }
    
    [OnDeserialized]
    protected void OnDeserializedTaskSuperMatchEntity(StreamingContext context)
    {
        if (CountToMatch <= 0)
        {
            CountToMatch = Value1;
        }
    }
    
#endregion
    
    public override void ConnectToBoard()
    {
        BoardService.Current.FirstBoard.BoardEvents.AddListener(this, GameEventsCodes.Match);
    }

    public override void DisconnectFromBoard()
    {
        BoardService.Current.FirstBoard.BoardEvents.RemoveListener(this, GameEventsCodes.Match); 
    }

    public void OnBoardEvent(int code, object context)
    {
        if (!IsInProgress())
        {
            return;
        }

        if (code != GameEventsCodes.Match)
        {
            return;
        }
        
        var matchDescr = context as MatchDescription;
        if (matchDescr == null)
        {
            Debug.LogError("[TaskMatchEntity] => OnBoardEvent: MatchDescription is null for GameEventsCodes.Match event");
            return;
        }

        if (PieceId != PieceType.None.Id && matchDescr.SourcePieceType != PieceId)
        {
            return;
        }
        
        if (matchDescr.MatchedPiecesCount >= CountToMatch)
        {
            CurrentValue += (int)(matchDescr.MatchedPiecesCount / CountToMatch);
        }
    }
}