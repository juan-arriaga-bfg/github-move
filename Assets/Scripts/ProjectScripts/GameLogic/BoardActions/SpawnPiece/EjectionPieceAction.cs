using System;
using System.Collections.Generic;
using UnityEngine;

public class EjectionPieceAction : IBoardAction
{
	public static readonly int ComponentGuid = ECSManager.GetNextGuid();
	public virtual int Guid => ComponentGuid;

	public BoardPosition From { get; set; }
	
	public Dictionary<int, int> Pieces { get; set; }
	
	public Action OnComplete { get; set; }
	
	public bool PerformAction(BoardController gameBoardController)
	{
		var pieces = new Dictionary<BoardPosition, Piece>();

		var positionsForLock = new List<BoardPosition>();
		
		foreach (var pair in Pieces)
		{
			var field = new List<BoardPosition>();
        
			if (gameBoardController.BoardLogic.EmptyCellsFinder.FindRandomNearWithPointInCenter(From, field, pair.Value) == false)
			{
				break;
			}
			
			foreach (var pos in field)
			{
				var piece = gameBoardController.CreatePieceFromType(pair.Key);

				if (gameBoardController.BoardLogic.AddPieceToBoard(pos.X, pos.Y, piece) == false)
				{
					continue;
				}
			
				pieces.Add(pos, piece);
				positionsForLock.Add(pos);
			}
		}
		
		gameBoardController.BoardLogic.LockCells(positionsForLock, this);
		gameBoardController.BoardLogic.LockCell(From, this);
		
		var animation = new ReproductionPieceAnimation
		{
			From = From,
			Pieces = pieces
		};

		animation.OnCompleteEvent += (_) =>
		{
			gameBoardController.BoardLogic.UnlockCell(From, this);

			foreach (var pair in pieces)
			{
				gameBoardController.BoardLogic.UnlockCell(pair.Key, this);
			}
//			
//			foreach (var piece in pieces.Values)
//			{
//				piece.PathfindLockObserver?.OnAddToBoard(piece.CachedPosition);
//			}

			gameBoardController.PathfindLocker.OnAddComplete();
			OnComplete?.Invoke();
		};
		
		gameBoardController.RendererContext.AddAnimationToQueue(animation);

		return true;
	}
}