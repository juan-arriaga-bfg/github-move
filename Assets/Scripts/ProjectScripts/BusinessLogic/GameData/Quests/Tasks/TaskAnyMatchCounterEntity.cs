using Quests;

public class TaskAnyMatchCounterEntity : TaskCounterEntity, IBoardEventListener, IConnectedToBoardEvent
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();
    public override int Guid => ComponentGuid;

    public void ConnectToBoard()
    {
        BoardService.Current.FirstBoard.BoardEvents.AddListener(this, GameEventsCodes.Match);
    }

    public void DisconnectFromBoard()
    {
        BoardService.Current.FirstBoard.BoardEvents.RemoveListener(this, GameEventsCodes.Match); 
    }

    public void OnBoardEvent(int code, object context)
    {
        if (!IsInProgress())
        {
            return;
        }
        
        if (code == GameEventsCodes.Match)
        {
            CurrentValue += 1;
        }
    }
}