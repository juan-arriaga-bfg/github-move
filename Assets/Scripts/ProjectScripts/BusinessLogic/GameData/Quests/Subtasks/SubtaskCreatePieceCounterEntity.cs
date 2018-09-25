using Newtonsoft.Json;
using Quests;

public class SubtaskCreatePieceCounterEntity : SubtaskCounterEntity, IBoardEventListener
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();
    public override int Guid => ComponentGuid;

    [JsonProperty] public int PieceId { get; protected set; }
    
    public override void OnRegisterEntity(ECSEntity entity)
    {
        BoardService.Current.FirstBoard.BoardEvents.AddListener(this, GameEventsCodes.CreatePiece);
    }

    public override void OnUnRegisterEntity(ECSEntity entity)
    {
        BoardService.Current.FirstBoard.BoardEvents.RemoveListener(this, GameEventsCodes.CreatePiece); 
    }

    public void OnBoardEvent(int code, object context)
    {
        if (State != SubtaskState.InProgress)
        {
            return;
        }
        
        if (code == GameEventsCodes.Match && PieceId == (int)context)
        {
            CurrentValue += 1;
        }
    }
}