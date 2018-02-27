using System.Collections.Generic;

public class TouchReactionDefinitionSpawnPiece : TouchReactionDefinitionComponent
{
	public int SpawnPieceType;
    
	public override bool Make(BoardPosition position, Piece piece)
	{
		piece.Context.ActionExecutor.AddAction(new SpawnPiecesAction()
		{
			IsCheckMatch = false,
			At = position,
			Pieces = new List<int> {SpawnPieceType}
		});
        
		return true;
	}
}