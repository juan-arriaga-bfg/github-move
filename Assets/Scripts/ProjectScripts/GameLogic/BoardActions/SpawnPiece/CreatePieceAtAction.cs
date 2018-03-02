public class CreatePieceAtAction : IBoardAction
{
	public static readonly int ComponentGuid = ECSManager.GetNextGuid();

	public virtual int Guid
	{
		get { return ComponentGuid; }
	}
	
	public BoardPosition At { get; set; }
	
	public int PieceTypeId { get; set; }
	
	public CurrencyPair Resources { get; set; }
	
	public bool PerformAction(BoardController gameBoardController)
	{
		var piece = gameBoardController.CreatePieceFromType(PieceTypeId);

		At = new BoardPosition(At.X, At.Y, piece.Layer.Index);
		
		if (At.IsValid == false) return false;
		
		if (gameBoardController.BoardLogic.IsLockedCell(At)) return false;

		if (gameBoardController.BoardLogic.AddPieceToBoard(At.X, At.Y, piece) == false) return false;
		
		gameBoardController.BoardLogic.LockCell(At, this);
		
		AddResourses(piece);
		
		var animation = new SpawnPieceAtAnimation
		{
			CreatedPiece = piece,
			At = At
		};

		animation.OnCompleteEvent += (_) =>
		{
			gameBoardController.BoardLogic.UnlockCell(At, this);
		};
		
		gameBoardController.RendererContext.AddAnimationToQueue(animation);
		
		return true;
	}

	private void AddResourses(Piece piece)
	{
		if (Resources == null) return;
		
		var storage = piece.GetComponent<ResourceStorageComponent>(ResourceStorageComponent.ComponentGuid);

		if (storage == null || Resources.Amount == 0) return;
		
		storage.Resources = Resources;
	}
}