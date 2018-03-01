using System.Collections.Generic;

public class TouchReactionDefinitionSpawnPiece : TouchReactionDefinitionComponent
{
	public int SpawnPieceType;
	public int SpaunAmaunt;
	public CurrencyPair Resources;
    
	public override bool Make(BoardPosition position, Piece piece)
	{
		var pieces = new List<int>();

		for (int i = 0; i < SpaunAmaunt; i++)
		{
			pieces.Add(SpawnPieceType);
		}
		
		piece.Context.ActionExecutor.AddAction(new SpawnPiecesAction()
		{
			Resources = Resources,
			IsCheckMatch = false,
			At = position,
			Pieces = pieces
		});
        
		return true;
	}
}