using Debug = IW.Logger;
using UnityEngine;

[TaskHighlight(typeof(HighlightTaskOpenChest))]
[TaskHighlight(typeof(HighlightTaskPointToMarketButton))]
public class TaskOpenChestEntity : TaskCounterAboutPiece, IBoardEventListener, IHavePieceId
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();
    public override int Guid => ComponentGuid;
    
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
        
        var piece = context as Piece;
        if (piece == null)
        {
            Debug.LogError("[TaskOpenChestEntity] => OnBoardEvent: Chest is null for GameEventsCodes.OpenChest event");
            return;
        }

        var def = PieceType.GetDefById(piece.PieceType);
        if (def.Filter.Has(PieceTypeFilter.Bag))
        {
            return;
        }

        if (PieceId <= 0 || piece.PieceType == PieceId)
        {
            CurrentValue += 1;
        }
    }
}