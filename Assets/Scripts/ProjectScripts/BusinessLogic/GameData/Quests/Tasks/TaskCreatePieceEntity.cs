using Newtonsoft.Json;
using Quests;

[TaskHighlight(typeof(HighlightTaskPiece))]
public class TaskCreatePieceEntity : TaskCounterAboutPiece, IBoardEventListener, IHavePieceId
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();
    public override int Guid => ComponentGuid;
    
    public override void ConnectToBoard()
    {
        BoardService.Current.FirstBoard.BoardEvents.AddListener(this, GameEventsCodes.PieceBuilded);
    }

    public override void DisconnectFromBoard()
    {
        BoardService.Current.FirstBoard.BoardEvents.RemoveListener(this, GameEventsCodes.PieceBuilded); 
    }

    public void OnBoardEvent(int code, object context)
    {
        if (!IsInProgress())
        {
            return;
        }
        
        if (code == GameEventsCodes.PieceBuilded && PieceId == (int)context)
        {
            CurrentValue += 1;
        }
    }
    
    public override string ToString()
    {
        string ret = $"{GetType()} [{Id}], State: {State}, Progress: {PieceType.GetDefById(PieceId).Abbreviations[0]} - {CurrentValue}/{TargetValue}";
        return ret;
    }
}