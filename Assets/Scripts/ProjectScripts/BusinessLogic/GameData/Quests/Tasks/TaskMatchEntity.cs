using Quests;
using UnityEngine;

public class TaskMatchEntity : TaskCounterEntity, IBoardEventListener, IHavePieceId
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();
    public override int Guid => ComponentGuid;

    public int PieceId { get; protected set; }
    
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

        if (PieceId == PieceType.None.Id || matchDescr.CreatedPieceType == PieceId)
        {
            CurrentValue += 1;
        }
    }
}