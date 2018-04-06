using System.Collections.Generic;

public class TouchReactionDefinitionSpawnInStorageAndRegress : TouchReactionDefinitionSpawnInStorage
{
    public override bool Make(BoardPosition position, Piece piece)
    {
        if (base.Make(position, piece) == false) return false;

        var previous = piece.Context.BoardLogic.MatchDefinition.GetPrevious(piece.PieceType);
        
        var spawn = new CreatePieceAtAction
        {
            At = position,
            PieceTypeId = previous
        };
        
        piece.Context.ActionExecutor.AddAction(new CollapsePieceToAction
        {
            To = position,
            Positions = new List<BoardPosition>{position},
            OnCompleteAction = spawn
        });
        
        return true;
    }
}