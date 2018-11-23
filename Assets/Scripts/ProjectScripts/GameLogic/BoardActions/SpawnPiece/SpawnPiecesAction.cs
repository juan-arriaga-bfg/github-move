using System;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPiecesAction : IBoardAction
{
	public static readonly int ComponentGuid = ECSManager.GetNextGuid();
	public virtual int Guid => ComponentGuid;

	public bool IsCheckMatch { get; set; }
	public bool IsMatch = false;
	public BoardPosition At { get; set; }
	
	public List<int> Pieces { get; set; }
	
	public Action<List<BoardPosition>> OnSuccessEvent { get; set; }

	public bool PerformAction(BoardController gameBoardController)
	{
		var free = new List<BoardPosition>();
		var pieces = Pieces;
		
		pieces.Sort();
		
		if (gameBoardController.BoardLogic.EmptyCellsFinder.FindRandomNearWithPointInCenter(At, free, Pieces.Count) == false)
		{
			return false;
		}
		
		var index = free.IndexOf(At);

		free.RemoveAt(index != -1 ? index : 0);
		free.Add(At);

		var positionsForLock = new List<BoardPosition>();
		
		for (var i = 0; i < pieces.Count; i++)
		{
			Action onSuccess = () => { };

			if (i == pieces.Count - 1 ) onSuccess = () =>
			{
				OnSuccessEvent?.Invoke(free);
			};

			var targetPos = free[i];
			targetPos.Z = gameBoardController.BoardDef.PieceLayer;
			
			positionsForLock.Add(targetPos);
			SpawnSinglePiece(gameBoardController, IsCheckMatch, IsMatch, targetPos, pieces[i], position =>
			{
				gameBoardController.BoardLogic.UnlockCell(targetPos, this);
				onSuccess();
			});
			
//			gameBoardController.ActionExecutor.AddAction(new SpawnPieceAtAction
//			{
//				IsCheckMatch = IsCheckMatch,
//				IsMatch = IsMatch,
//				At = free[i],
//				PieceTypeId = pieces[i],
//				OnSuccessEvent = position => { onSuccess(); }
//			});
		}
		gameBoardController.BoardLogic.LockCells(positionsForLock, this);
		
		return true;
	}

	private bool SpawnSinglePiece(BoardController gameBoardController, bool IsCheckMatch, bool IsMatch, BoardPosition At,
								  int PieceTypeId, Action<BoardPosition> OnSuccessEvent)
	{
		var piece = gameBoardController.CreatePieceFromType(PieceTypeId);
		
		At = new BoardPosition(At.X, At.Y, piece.Layer.Index);

		if (At.IsValid == false
		    || gameBoardController.BoardLogic.IsLockedCell(At)
		    || gameBoardController.BoardLogic.AddPieceToBoard(At.X, At.Y, piece) == false)
		{
			return false;
		}

		BoardAnimation animation;
		
		if (IsMatch)
		{
			animation = new MatchSpawnPieceAtAnimation
			{
				CreatedPiece = piece,
				At = At
			};
		}
		else
		{
			animation = new SpawnPieceAtAnimation
			{
				CreatedPiece = piece,
				At = At
			};
		}
		
		animation.OnCompleteEvent += (_) =>
		{
			if (OnSuccessEvent == null) return;
			
			var observer = piece.GetComponent<MulticellularPieceBoardObserver>(MulticellularPieceBoardObserver.ComponentGuid);
				
			OnSuccessEvent(observer?.GetTopPosition ?? At);
		};
		
		gameBoardController.RendererContext.AddAnimationToQueue(animation);

		return true;
	}
	
	
}