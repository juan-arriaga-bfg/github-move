using System.Collections.Generic;

public class TouchReactionDefinitionSpawnInStorage : TouchReactionDefinitionComponent
{
    public override bool Make(BoardPosition position, Piece piece)
    {
        var storage = piece.GetComponent<StorageComponent>(StorageComponent.ComponentGuid);

        if (storage == null) return false;

        int amount;

        if (storage.Scatter(out amount) == false)
        {
            return false;
        }
        
        var free = new List<BoardPosition>();
        var positions = new List<BoardPosition>();

        if (piece.Context.BoardLogic.EmptyCellsFinder.FindRandomNearWithPointInCenter(position, free, amount * 5, 7) == false)
        {
            return false;
        }
        
        foreach (var pos in free)
        {
            positions.Add(pos);
            
            if(positions.Count == amount) break;
        }
        
        piece.Context.ActionExecutor.AddAction(new ReproductionPieceAction
        {
            From = position,
            Piece = storage.SpawnPiece,
            Positions = positions
        });
        
        return true;
    }
}