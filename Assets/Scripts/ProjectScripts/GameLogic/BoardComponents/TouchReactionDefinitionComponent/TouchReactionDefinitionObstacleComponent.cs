using System;
using System.Collections.Generic;

public class TouchReactionDefinitionObstacleComponent : TouchReactionDefinitionSpawnInStorage
{
    public override bool Make(BoardPosition position, Piece piece)
    {
        var storageLife = piece.GetComponent<StorageLifeComponent>(StorageLifeComponent.ComponentGuid);
        var storage = piece.GetComponent<StorageComponent>(StorageComponent.ComponentGuid);

        if (storageLife.IsDead == false) return base.Make(position, piece);
        
        var action = storage.SpawnAction as EjectionPieceAction;

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
        
        UIErrorWindowController.AddError("Production of the resource is not complete!");
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