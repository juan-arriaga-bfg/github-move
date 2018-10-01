using UnityEngine;

public class TaskOpenChestEntity : TaskCounterEntity, IBoardEventListener, IHavePieceId
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();
    public override int Guid => ComponentGuid;

    public int PieceId { get; protected set; }
    
    public override void ConnectToBoard()
    {
        BoardService.Current.FirstBoard.BoardEvents.AddListener(this, GameEventsCodes.OpenChest);
    }

    public override void DisconnectFromBoard()
    {
        BoardService.Current.FirstBoard.BoardEvents.RemoveListener(this, GameEventsCodes.OpenChest); 
    }

    public void OnBoardEvent(int code, object context)
    {
        if (!IsInProgress())
        {
            return;
        }

        if (code != GameEventsCodes.OpenChest)
        {
            return;
        }
        
        var chest = context as Chest;
        if (chest == null)
        {
            Debug.LogError("[TaskOpenChestEntity] => OnBoardEvent: Chest is null for GameEventsCodes.OpenChest event");
            return;
        }

        if (PieceId == PieceType.None.Id || chest.Piece == PieceId)
        {
            CurrentValue += 1;
        }
    }
}