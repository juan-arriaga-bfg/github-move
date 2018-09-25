using Newtonsoft.Json;
using Quests;

public class TaskCreatePieceCounterEntity : TaskCounterEntity, IBoardEventListener, IConnectedToBoardEvent
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();
    public override int Guid => ComponentGuid;

    [JsonProperty] public int PieceId { get; protected set; }
    
    public void ConnectToBoard()
    {
        BoardService.Current.FirstBoard.BoardEvents.AddListener(this, GameEventsCodes.CreatePiece);
    }

    public void DisconnectFromBoard()
    {
        BoardService.Current.FirstBoard.BoardEvents.RemoveListener(this, GameEventsCodes.CreatePiece); 
    }

    public void OnBoardEvent(int code, object context)
    {
        if (!IsInProgress())
        {
            return;
        }
        
        if (code == GameEventsCodes.CreatePiece && PieceId == (int)context)
        {
            CurrentValue += 1;
        }
    }
}