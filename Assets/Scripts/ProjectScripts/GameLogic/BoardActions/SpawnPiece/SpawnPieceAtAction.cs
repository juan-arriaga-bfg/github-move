using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPieceAtAction : IBoardAction
{
	public static readonly int ComponentGuid = ECSManager.GetNextGuid();

	public virtual int Guid
	{
		get { return ComponentGuid; }
	}


	public BoardPosition At { get; set; }
	
	public int PieceTypeId { get; set; }

	public bool PerformAction(BoardController gameBoardController)
	{
		Piece piece = gameBoardController.CreatePieceFromType(PieceTypeId);

		At = new BoardPosition(At.X, At.Y, piece.Layer.Index);
		
		if (At.IsValid == false) return false;
		
		if (gameBoardController.BoardLogic.IsLockedCell(At)) return false;

		if (gameBoardController.BoardLogic.AddPieceToBoard(At.X, At.Y, piece) == false) return false;
		
		gameBoardController.BoardLogic.LockCell(At, this);
		
		var animation = new SpawnPieceAtAnimation
		{
			Action = this
		};

		animation.OnCompleteEvent += (_) =>
		{
			gameBoardController.BoardLogic.UnlockCell(At, this);
			
			gameBoardController.ActionExecutor.AddAction(new CheckMatchAction
			{
				At = At
			});
		};
		
		gameBoardController.RendererContext.AddAnimationToQueue(animation);
		
		return true;
	}
}

