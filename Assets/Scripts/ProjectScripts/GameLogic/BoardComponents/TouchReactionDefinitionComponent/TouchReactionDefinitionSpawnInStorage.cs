using System.Collections.Generic;

public class TouchReactionDefinitionSpawnInStorage : TouchReactionDefinitionComponent
{
    public bool IsAutoStart = true;

    private int amount;
    
    public override bool IsViewShow(ViewDefinitionComponent viewDefinition)
    {
        return viewDefinition != null && viewDefinition.AddView(ViewType.StorageState).IsShow;
    }
    
    public override bool Make(BoardPosition position, Piece piece)
    {
        var storage = piece.GetComponent<StorageComponent>(StorageComponent.ComponentGuid);

        if (storage == null) return false;
        
        if(!storage.IsSpawnResource && !piece.Context.BoardLogic.EmptyCellsFinder.CheckFreeSpaceNearPosition(position, storage.Filling))
        {
            UIErrorWindowController.AddError(LocalizationService.Get("message.error.freeSpace", "message.error.freeSpace"));
            return false;
        }
        
        storage.OnHideBubble = () => { Spawn(position, piece, storage); };

        if (storage.Scatter(out amount, IsAutoStart)) return true;
        
        UIErrorWindowController.AddError(LocalizationService.Get("message.error.notComplete", "message.error.notComplete"));
        return false;
    }

    private void Spawn(BoardPosition position, Piece piece, StorageComponent storage)
    {
        if (storage == null) return;
        
        storage.OnHideBubble = null;

        if (storage.IsSpawnResource)
        {
            AddResourceView.Show(position, new CurrencyPair{Currency = Currency.GetCurrencyDef(storage.SpawnPiece).Name, Amount = amount});
            return;
        }
        
        var free = new List<BoardPosition>();
        var positions = new List<BoardPosition>();

        if (piece.Context.BoardLogic.EmptyCellsFinder.FindRandomNearWithPointInCenter(position, free, amount) == false)
        {
            return;
        }

        foreach (var pos in free)
        {
            positions.Add(pos);
            if (positions.Count == amount) break;
        }
        
        if (storage.SpawnAction != null)
        {
            piece.Context.ActionExecutor.AddAction(storage.SpawnAction);
            storage.SpawnAction = null;
            return;
        }
        
        piece.Context.ActionExecutor.AddAction(new ReproductionPieceAction
        {
            From = position,
            Piece = storage.SpawnPiece,
            Positions = positions,
            AnimationResourceSearch = pieceType => AnimationDataManager.FindAnimation(pieceType, def => def.OnPurchaseSpawn),
            OnComplete = () => { storage.OnScatter?.Invoke(); }
        });
    }
}