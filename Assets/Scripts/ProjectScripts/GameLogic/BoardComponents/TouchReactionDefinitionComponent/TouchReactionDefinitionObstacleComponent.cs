using System;
using System.Collections.Generic;
using System.Linq;

public class TouchReactionDefinitionObstacleComponent : TouchReactionDefinitionSpawnInStorage
{
    public override bool Make(BoardPosition position, Piece piece)
    {
        var life = piece.GetComponent<StorageLifeComponent>(StorageLifeComponent.ComponentGuid);
        var storage = piece.GetComponent<StorageComponent>(StorageComponent.ComponentGuid);
        
        var action = storage.SpawnAction as EjectionPieceAction;
        var amount = action?.Pieces.Sum(pair => pair.Value) ?? 0;
        
        if (action != null && !piece.Context.BoardLogic.EmptyCellsFinder.CheckFreeSpaceNearPosition(position, amount))
        {
            UIErrorWindowController.AddError(LocalizationService.Instance.Manager.GetTextByUid("message.error.freeSpace", "Free space not found!"));
            return false;
        }
        
        if (life.IsDead == false) return base.Make(position, piece);
        
        if (action != null)
        {
            var onComplete = action.OnComplete;
            
            action.OnComplete = () => CollapsePiece(position, piece, storage, () => { onComplete?.Invoke(); });
            
            return base.Make(position, piece);
        }
        
        storage.OnHideBubble = () =>
        {
            storage.OnHideBubble = null;
            CollapsePiece(position, piece, storage, () => { storage.OnScatter?.Invoke(); });
        };
        
        int tmp;
        if (storage.Scatter(out tmp, IsAutoStart)) return true;
        
        UIErrorWindowController.AddError(LocalizationService.Instance.Manager.GetTextByUid("message.error.notComplete", "Production of the resource is not complete!"));
        return false;
    }

    private void CollapsePiece(BoardPosition position, Piece piece, StorageComponent storage, Action onComplete)
    {
        piece.Context.ActionExecutor.AddAction(new CollapsePieceToAction
        {
            Positions = new List<BoardPosition> {position},
            To = position,
            OnCompleteAction = new CreatePieceAtAction
            {
                At = position,
                PieceTypeId = storage.SpawnPiece,
                OnComplete = onComplete
            }
        });
    }
}