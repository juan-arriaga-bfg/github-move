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
        
        var pieces = new List<int>();

        for (int i = 0; i < amount; i++)
        {
            pieces.Add(storage.SpawnPiece);
        }
        
        piece.Context.ActionExecutor.AddAction(new SpawnPiecesAction()
        {
            IsCheckMatch = false,
            IsShuffle = true,
            At = position,
            Pieces = pieces
        });
        
        return true;
    }
}