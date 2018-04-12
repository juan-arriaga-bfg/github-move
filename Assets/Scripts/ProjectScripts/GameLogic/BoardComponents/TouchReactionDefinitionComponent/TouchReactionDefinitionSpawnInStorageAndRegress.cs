using System.Collections.Generic;

public class TouchReactionDefinitionSpawnInStorageAndRegress : TouchReactionDefinitionComponent
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

        if (piece.Context.BoardLogic.EmptyCellsFinder.FindRandomNearWithPointInCenter(position, free, amount * 5, 7) == false)
        {
            return false;
        }

        for (var i = 0; i < free.Count; i++)
        {
            if (i == amount) break;
            
            var pos = free[i];
            
            piece.Context.ActionExecutor.AddAction(new SpawnPiceAndJumpAction
            {
                Piece = storage.SpawnPiece,
                From = position,
                To = free[i]
            });
        }
        
        var previous = piece.Context.BoardLogic.MatchDefinition.GetPrevious(piece.PieceType);
        
        var regress = new CreatePieceAtAction
        {
            At = position,
            PieceTypeId = previous
        };
        
        piece.Context.ActionExecutor.AddAction( new CollapsePieceToAction
        {
            To = position,
            Positions = new List<BoardPosition>{position},
            OnCompleteAction = regress
        });
        
        return true;
    }
}