using Debug = IW.Logger;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using UnityEngine;

[TaskHighlight(typeof(HighlightTaskSuperMatch))]
public class TaskSuperMatchEntity : TaskCounterAboutPiece, IBoardEventListener
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();
    public override int Guid => ComponentGuid;

    public int CountToMatch { get; protected set; }
    
    [JsonProperty] protected int Value1 { get; private set; }
    
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

        if (PieceId != PieceType.None.Id && PieceId != PieceType.Empty.Id && matchDescr.SourcePieceType != PieceId)
        {
            return;
        }
        
        if (matchDescr.SourcePiecesCount >= CountToMatch)
        {
            CurrentValue += (int)(matchDescr.SourcePiecesCount / CountToMatch);
        }
    }

    public override string GetLocalizedMessage()
    {
        var ret = base.GetLocalizedMessage();
        ret = ret.Replace("{0}", CountToMatch.ToString());

        return ret;
    }
}