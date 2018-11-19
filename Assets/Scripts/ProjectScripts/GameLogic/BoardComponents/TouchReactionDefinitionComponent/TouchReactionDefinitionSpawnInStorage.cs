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
            UIErrorWindowController.AddError(LocalizationService.Instance.Manager.GetTextByUid("message.error.freeSpace", "Free space not found!"));
            return false;
        }
        
        storage.OnHideBubble = () => { Spawn(position, piece, storage); };

        if (storage.Scatter(out amount, IsAutoStart)) return true;
        
        UIErrorWindowController.AddError(LocalizationService.Instance.Manager.GetTextByUid("message.error.notComplete", "Production of the resource is not complete!"));
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
            OnComplete = () => { storage.OnScatter?.Invoke(); }
        });
    }
}