using System.Collections.Generic;

public class TouchReactionDefinitionSpawnPiece : TouchReactionDefinitionComponent
{
	public override bool Make(BoardPosition position, Piece piece)
	{
		var def = GameDataService.Current.PiecesManager.GetPieceDef(piece.PieceType);

		if (def == null)
		{
			return false;
		}
		
		if (CreateResource(piece.Context, position, def.SpawnResources) || CreatePieces(piece.Context, position, def))
		{
			return true;
		}

		return false;
	}

	private bool CreateResource(BoardController gameBoard, BoardPosition position, CurrencyPair resources)
	{
		if (resources == null)
		{
			return false;
		}
		
		var id = PieceType.Parse(resources.Currency);
		var piece = gameBoard.CreatePieceFromType(id);

		if (piece == null)
		{
			return false;
		}
		
		var storage = piece.GetComponent<ResourceStorageComponent>(ResourceStorageComponent.ComponentGuid);

		if (storage == null || resources.Amount <= 0)
		{
			return false;
		}
		
//		storage.Resources = resources;
		
		gameBoard.ActionExecutor.AddAction(new SpawnResourcePieceAction()
		{
			At = position,
			Resource = piece
		});
		
		return true;
	}

	private bool CreatePieces(BoardController gameBoard, BoardPosition position, PieceDef def)
	{
		var id = def.SpawnPieceType;
		
		if (id == PieceType.None.Id || def.SpawnAmount <= 0)
		{
			return false;
		}
		
		var pieces = new List<int>();

		for (int i = 0; i < def.SpawnAmount; i++)
		{
			pieces.Add(id);
		}
		
		gameBoard.ActionExecutor.AddAction(new SpawnPiecesAction()
		{
			IsCheckMatch = false,
			At = position,
			Pieces = pieces
		});
        
		return true;
	}
}