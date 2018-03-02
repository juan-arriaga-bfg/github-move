using System;

public class SpawnPieceAtAction : IBoardAction
{
	public static readonly int ComponentGuid = ECSManager.GetNextGuid();

	public virtual int Guid
	{
		get { return ComponentGuid; }
	}
	
	public bool IsCheckMatch { get; set; }
	
	public BoardPosition At { get; set; }
	
	public int PieceTypeId { get; set; }
	
	public CurrencyPair Resources { get; set; }

	public Piece CreatedPiece { get; set; }

	public Action<SpawnPieceAtAction> OnFailedAction { get; set; }

	public bool PerformAction(BoardController gameBoardController)
	{
		var piece = gameBoardController.CreatePieceFromType(PieceTypeId);

		At = new BoardPosition(At.X, At.Y, piece.Layer.Index);

		if (At.IsValid == false)
		{
			if (OnFailedAction != null)
			{
				OnFailedAction(this);
			}
			return false;
		}

		if (gameBoardController.BoardLogic.IsLockedCell(At))
		{
			if (OnFailedAction != null)
			{
				OnFailedAction(this);
			}
			return false;
		}

		if (gameBoardController.BoardLogic.AddPieceToBoard(At.X, At.Y, piece) == false)
		{
			if (OnFailedAction != null)
			{
				OnFailedAction(this);
			}
			return false;
		}

		this.CreatedPiece = piece;
		
		gameBoardController.BoardLogic.LockCell(At, this);
		
		AddResourses(piece);
		
		var animation = new SpawnPieceAtAnimation
		{
			Action = this
		};

		animation.OnCompleteEvent += (_) =>
		{
			gameBoardController.BoardLogic.UnlockCell(At, this);

			if (IsCheckMatch)
			{
				gameBoardController.ActionExecutor.AddAction(new CheckMatchAction
				{
					At = At
				});
			}
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

