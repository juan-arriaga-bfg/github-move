using System;
using System.Runtime.Serialization;
using UnityEngine;

[TaskHighlight(typeof(HighlightTaskChestBranch))]
[TaskHighlight(typeof(HighlightTaskPointToMarketButton))]
public class TaskOpenChestTypeEntity : TaskCounterAboutPiece, IBoardEventListener, IHavePieceId
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();
    public override int Guid => ComponentGuid;
    
    private string targetBranch;
    
    public override void ConnectToBoard()
    {
        BoardService.Current.FirstBoard.BoardEvents.AddListener(this, GameEventsCodes.OpenChest);
    }

    public override void DisconnectFromBoard()
    {
        BoardService.Current.FirstBoard.BoardEvents.RemoveListener(this, GameEventsCodes.OpenChest); 
    }

    [OnDeserialized]
    protected void OnDeserializedTaskOpenChestTypeEntity(StreamingContext context)
    {
        if (!string.IsNullOrEmpty(PieceUid))
        {
            PieceId = PieceType.Parse(PieceUid);
            targetBranch = HighlightTaskPointToPieceSourceHelper.PieceBranchRegexComplex.Match(PieceUid).Value;
            
            if (string.IsNullOrEmpty(targetBranch))
            {
                Debug.LogError($"[TaskOpenChestTypeEntity] => TaskOpenChestTypeEntity: Branch for {PieceUid} is empty!");
            }
        }
        else
        {
            throw new Exception($"[TaskOpenChestTypeEntity] => TaskOpenChestTypeEntity: PieceUid is not defined");
        }
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
            IW.Logger.LogError("[TaskOpenChestEntity] => OnBoardEvent: Chest is null for GameEventsCodes.OpenChest event");
            return;
        }

        var def = PieceType.GetDefById(piece.PieceType);
        if (def.Filter.Has(PieceTypeFilter.Bag))
        {
            return;
        }

        if (def.Filter.Has(PieceTypeFilter.Chest))
        {
            string branch = HighlightTaskPointToPieceSourceHelper.PieceBranchRegexComplex.Match(def.Abbreviations[0]).Value;
            if (string.IsNullOrEmpty(branch))
            {
                IW.Logger.LogError($"[TaskOpenChestTypeEntity] => OnBoardEvent: Branch for {def.Abbreviations[0]} is empty!");
                return;
            }

            if (branch == targetBranch)
            {
                CurrentValue += 1; 
            }
        }
    }
}