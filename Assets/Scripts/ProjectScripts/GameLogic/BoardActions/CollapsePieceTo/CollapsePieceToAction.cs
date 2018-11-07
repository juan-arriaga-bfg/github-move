using System;
using System.Collections.Generic;
using UnityEngine;

public class CollapsePieceToAction : IBoardAction
{
	public static readonly int ComponentGuid = ECSManager.GetNextGuid();
	public virtual int Guid => ComponentGuid;

	public BoardPosition To { get; set; }

	public List<BoardPosition> Positions { get; set; }
	
	public IBoardAction OnCompleteAction;

	public Action OnComplete;

	public bool IsMatch = false;

	private List<BoardPosition> GetPositionsIncludeMask(BoardController board)
	{
		var resultPositions = new List<BoardPosition>();
		foreach (var pos in Positions)
		{
			var piece = board.BoardLogic.GetPieceAt(pos);
			if (piece.Multicellular != null)
			{
				foreach (var maskPosition in piece.Multicellular.Mask)
				{
					resultPositions.Add(piece.Multicellular.GetPointInMask(piece.CachedPosition, maskPosition));
				}
			}
			else
			{
				resultPositions.Add(pos);
			}
		}

		return resultPositions;
	}
	
	public bool PerformAction(BoardController gameBoardController)
	{
		if (Positions == null || Positions.Count == 0) return false;

		var positonsForLock = GetPositionsIncludeMask(gameBoardController);
			
		var targetPiece = gameBoardController.BoardLogic.GetPieceAt(To);
	
		var obstaclesPieces = new List<Piece>();
		
		foreach (var pos in Positions)
		{
			var piece = gameBoardController.BoardLogic.GetPieceAt(pos);
			if (((int)PieceType.GetDefById(piece.PieceType).Filter & (int)PieceTypeFilter.Obstacle) != 0)
			{
				obstaclesPieces.Add(piece);
			}	
		}
		
		gameBoardController.BoardLogic.RemovePiecesAt(Positions);
		gameBoardController.BoardLogic.LockCells(positonsForLock, this);
		
		BoardAnimation animation;
		
		if (IsMatch)
			animation = new MatchPieceToAnimation { Action = this };
		else
			animation = new CollapsePieceToAnimation { Action = this };
		
		
		animation.OnCompleteEvent += (_) =>
		{
			foreach (var obstaclePiece in obstaclesPieces)
			{
				obstaclePiece.PathfindLockObserver.RemoveRecalculate(obstaclePiece.CachedPosition);
			}
			
			gameBoardController.BoardLogic.UnlockCells(positonsForLock, this);
			
			if (OnCompleteAction != null) gameBoardController.ActionExecutor.AddAction(OnCompleteAction);
			OnComplete?.Invoke();
		};
		
		gameBoardController.RendererContext.AddAnimationToQueue(animation);
		
		return true;
	}
}