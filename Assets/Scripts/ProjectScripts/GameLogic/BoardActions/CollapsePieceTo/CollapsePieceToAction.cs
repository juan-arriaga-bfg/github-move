using System;
using System.Collections.Generic;
using UnityEngine;

public class CollapsePieceToAction : IBoardAction
{
	public static readonly int ComponentGuid = ECSManager.GetNextGuid();
	
	public virtual int Guid
	{
		get { return ComponentGuid; }
	}
	
	public BoardPosition To { get; set; }

	public List<BoardPosition> Positions { get; set; }
	
	public IBoardAction OnCompleteAction;

	public Action OnComplete;

	public bool IsMatch = false;
	
	public bool PerformAction(BoardController gameBoardController)
	{
		if (Positions == null || Positions.Count == 0) return false;
		
		gameBoardController.BoardLogic.LockCells(Positions, this);
		
		gameBoardController.BoardLogic.RemovePiecesAt(Positions);

		BoardAnimation animation;
		if (IsMatch)
		{
			animation = new MatchPieceToAnimation
			{
				Action = this
			};
		}
		else
		{
			animation = new CollapsePieceToAnimation
			{
				Action = this
			};
		}
		 

		animation.OnCompleteEvent += (_) =>
		{
			gameBoardController.BoardLogic.UnlockCells(Positions, this);
			if (OnCompleteAction != null) gameBoardController.ActionExecutor.AddAction(OnCompleteAction);
			if (OnComplete != null) OnComplete();
		};
		
		gameBoardController.RendererContext.AddAnimationToQueue(animation);
		
		return true;
	}
}