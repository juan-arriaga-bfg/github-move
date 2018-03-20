using System.Collections.Generic;
using UnityEngine;

public class TouchReactionDefinitionSpawnInStorage : TouchReactionDefinitionComponent
{
    public override bool Make(BoardPosition position, Piece piece)
    {
        var storage = piece.GetComponent<StorageComponent>(StorageComponent.ComponentGuid);

        if (storage == null) return false;

        int id;
        int amount;

        if (storage.Scatter(out id, out amount) == false)
        {
            return false;
        }
        
        var pieces = new List<int>();

        for (int i = 0; i < amount; i++)
        {
            pieces.Add(id);
        }
        
        Debug.LogError(pieces.Count);
		
        piece.Context.ActionExecutor.AddAction(new SpawnPiecesAction()
        {
            IsCheckMatch = false,
            At = position,
            Pieces = pieces
        });
        
        return true;
    }
}