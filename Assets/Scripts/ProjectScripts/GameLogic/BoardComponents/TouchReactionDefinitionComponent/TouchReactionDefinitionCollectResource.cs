using System.Collections.Generic;

public class TouchReactionDefinitionCollectResource : TouchReactionDefinitionComponent
{
    public override bool Make(BoardPosition position, Piece piece)
    {
        var storage = piece.GetComponent<ResourceStorageComponent>(ResourceStorageComponent.ComponentGuid);

        if (storage == null || storage.Resources == null || storage.Resources.Amount == 0) return false;

        AddResourceView.Show(position, storage.Resources);
        
        piece.Context.ActionExecutor.AddAction(new CollapsePieceToAction
        {
            To = position,
            Positions = new List<BoardPosition>{position}
        });
        
        return true;
    }
}