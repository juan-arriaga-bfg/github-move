using System.Collections.Generic;

public class TouchReactionDefinitionSpawnInStorage : TouchReactionDefinitionComponent
{
    public bool IsAutoStart = true;
    
    public override bool IsViewShow(ViewDefinitionComponent viewDefinition)
    {
        return viewDefinition != null && viewDefinition.AddView(ViewType.StorageState).IsShow;
    }
    
    public override bool Make(BoardPosition position, Piece piece)
    {
        var storage = piece.GetComponent<StorageComponent>(StorageComponent.ComponentGuid);

        if (storage == null) return false;

        int amount;
        
        
        if(!piece.Context.BoardLogic.EmptyCellsFinder.CheckFreeSpaceNearPosition(position, storage.Filling))
        {
            UIErrorWindowController.AddError("Free space not found");
            return false;
        }
        
        if (storage.Scatter(out amount, IsAutoStart) == false)
        {
            UIErrorWindowController.AddError("Production of the resource is not complete!");
            return false;
        }
        
        var free = new List<BoardPosition>();
        var positions = new List<BoardPosition>();

        if(!piece.Context.BoardLogic.EmptyCellsFinder.FindRandomNearWithPointInCenter(position, free, amount))
        {
            return false;
        }

        foreach (var pos in free)
        {
            positions.Add(pos);
            if(positions.Count == amount) break;
        }
        
        if (storage.SpawnAction != null)
        {
            piece.Context.ActionExecutor.AddAction(storage.SpawnAction);
            storage.SpawnAction = null;
            return true;
        }
        
        piece.Context.ActionExecutor.AddAction(new ReproductionPieceAction
        {
            From = position,
            Piece = storage.SpawnPiece,
            Positions = positions,
            OnComplete = () =>
            {
                if (storage.OnScatter != null) storage.OnScatter();
            }
        });
        
        return true;
    }
}