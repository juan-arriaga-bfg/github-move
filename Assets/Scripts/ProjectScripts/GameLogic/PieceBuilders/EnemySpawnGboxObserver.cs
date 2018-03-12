using System.Collections.Generic;

public class EnemySpawnGboxObserver : IECSComponent, IPieceBoardObserver
{
	public static readonly int ComponentGuid = ECSManager.GetNextGuid();

	public int Guid
	{
		get { return ComponentGuid; }
	}

	private Piece contextPiece;
	
	private List<BoardPosition> cachedGboxPositions = new List<BoardPosition>();

	public void OnRegisterEntity(ECSEntity entity)
	{
		contextPiece = entity as Piece;

		if (contextPiece != null)
		{
			var observer = contextPiece.GetComponent<PieceBoardObserversComponent>(PieceBoardObserversComponent.ComponentGuid);
			if (observer != null)
			{
				observer.RegisterObserver(this);
			}
		}
	}

	public void OnUnRegisterEntity(ECSEntity entity)
	{
		if (contextPiece != null)
		{
			var observer = contextPiece.GetComponent<PieceBoardObserversComponent>(PieceBoardObserversComponent.ComponentGuid);
			if (observer != null)
			{
				observer.UnRegisterObserver(this);
			}
		}
	}

	public void OnAddToBoard(BoardPosition position, Piece context = null)
	{
		if (contextPiece != null)
		{
			contextPiece.Context.ActionExecutor.PerformAction(new SpawnPiecesAction{ At = position, Pieces = new List<int>
			{
				PieceType.Gbox1.Id
			},
				OnSuccessEvent = list =>
				{
					cachedGboxPositions = new List<BoardPosition>(list);
				}
			});
		}
	}

	public void OnMovedFromTo(BoardPosition @from, BoardPosition to, Piece context = null)
	{
		
	}

	public void OnRemoveFromBoard(BoardPosition position, Piece context = null)
	{
		for (int i = 0; i < cachedGboxPositions.Count; i++)
		{
			var gboxPos = cachedGboxPositions[i];
			contextPiece.Context.ActionExecutor.PerformAction(new CollapsePieceToAction
			{
				Positions = new List<BoardPosition>
				{
					gboxPos
				}
			});
		}
	}

}