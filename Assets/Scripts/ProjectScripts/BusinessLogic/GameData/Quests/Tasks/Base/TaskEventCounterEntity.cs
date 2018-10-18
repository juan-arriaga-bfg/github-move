using Newtonsoft.Json;

/// <summary>
/// Base class for Tasks that counts BoardEvent occurrences
/// </summary>
public abstract class TaskEventCounterEntity : TaskCounterEntity, IBoardEventListener
{
    [JsonIgnore]
    protected abstract int EventCode { get; }

    public override void ConnectToBoard()
    {
        BoardService.Current.FirstBoard.BoardEvents.AddListener(this, EventCode);
    }

    public override void DisconnectFromBoard()
    {
        BoardService.Current.FirstBoard.BoardEvents.RemoveListener(this, EventCode); 
    }

    public virtual void OnBoardEvent(int code, object context)
    {
        if (!IsInProgress())
        {
            return;
        }

        if (code != EventCode)
        {
            return;
        }
        
        CurrentValue += 1;
    }
}