
using System.Collections.Generic;

public class SpawnResourcePieceAction : IBoardAction
{
	public static readonly int ComponentGuid = ECSManager.GetNextGuid();
	
	public virtual int Guid
	{
		get { return ComponentGuid; }
	}
	
	public BoardPosition At { get; set; }
	
	public Piece Resource { get; set; }
	
	public bool PerformAction(BoardController gameBoardController)
	{
		var free = new List<BoardPosition>();

		if (gameBoardController.BoardLogic.EmptyCellsFinder.FindNearWithPointInCenter(At, free, 1, 5) == false)
		{
			return false;
		}

		var index = free.IndexOf(At);

		if (index != -1)
		{
			free.RemoveAt(index);
			free.Add(At);
		}

		At = free[0];
		
		At = new BoardPosition(At.X, At.Y, Resource.Layer.Index);
		
		if (At.IsValid == false
		    || gameBoardController.BoardLogic.IsLockedCell(At)
		    || gameBoardController.BoardLogic.AddPieceToBoard(At.X, At.Y, Resource) == false)
		{
			return false;
		}
		
		gameBoardController.BoardLogic.LockCell(At, this);
		
		var animation = new SpawnPieceAtAnimation
		{
			CreatedPiece = Resource,
			At = At
		};

		animation.OnCompleteEvent += (_) =>
		{
			gameBoardController.BoardLogic.UnlockCell(At, this);
		};
		
		gameBoardController.RendererContext.AddAnimationToQueue(animation);
		
		return true;
	}
}