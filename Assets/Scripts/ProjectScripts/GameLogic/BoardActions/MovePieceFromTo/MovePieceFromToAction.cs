using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePieceFromToAction : IBoardAction
{
	public static readonly int ComponentGuid = ECSManager.GetNextGuid();

	public virtual int Guid
	{
		get { return ComponentGuid; }
	}

	public BoardPosition From { get; set; }
	
	public BoardPosition To { get; set; }
	
	public bool PerformAction(BoardController gameBoardController)
	{
		To = new BoardPosition(To.X, To.Y, From.Z);
		
		bool state = (gameBoardController.BoardLogic.IsLockedCell(To) == false)
		             && (gameBoardController.BoardLogic.IsEmpty(To))
		             && (gameBoardController.BoardLogic.MovePieceFromTo(From, To));

		if (state == false)
		{
			
			return false;
		}
		
		gameBoardController.BoardLogic.LockCell(From, this);
		gameBoardController.BoardLogic.LockCell(To, this);
		
		var animation = new MovePieceFromToAnimation
		{
			Action = this
		};

		animation.OnCompleteEvent += (_) =>
		{
			gameBoardController.BoardLogic.UnlockCell(From, this);
			gameBoardController.BoardLogic.UnlockCell(To, this);
		};
		
		gameBoardController.RendererContext.AddAnimationToQueue(animation);
		
		return true;
	}
}

