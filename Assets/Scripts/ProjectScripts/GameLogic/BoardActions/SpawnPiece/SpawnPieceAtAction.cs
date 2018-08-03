﻿using System;

public class SpawnPieceAtAction : IBoardAction
{
	public static readonly int ComponentGuid = ECSManager.GetNextGuid();

	public virtual int Guid
	{
		get { return ComponentGuid; }
	}
	
	public bool IsCheckMatch { get; set; }
	public bool IsMatch = false;
	
	public BoardPosition At { get; set; }
	
	public int PieceTypeId { get; set; }
	
	public Action<SpawnPieceAtAction> OnFailedAction { get; set; }
	public Action<BoardPosition> OnSuccessEvent { get; set; }

	public bool PerformAction(BoardController gameBoardController)
	{
		var piece = gameBoardController.CreatePieceFromType(PieceTypeId);

		At = new BoardPosition(At.X, At.Y, piece.Layer.Index);

		if (At.IsValid == false
		    || gameBoardController.BoardLogic.IsLockedCell(At)
		    || gameBoardController.BoardLogic.AddPieceToBoard(At.X, At.Y, piece) == false)
		{
			if (OnFailedAction != null)
			{
				OnFailedAction(this);
			}
			return false;
		}
		
		gameBoardController.BoardLogic.LockCell(At, this);

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
			gameBoardController.BoardLogic.UnlockCell(At, this);

			if (IsCheckMatch)
			{
				gameBoardController.ActionExecutor.AddAction(new CheckMatchAction
				{
					At = At
				});
			}

			if (OnSuccessEvent != null)
			{
				var observer = piece.GetComponent<MulticellularPieceBoardObserver>(MulticellularPieceBoardObserver.ComponentGuid);
				
				OnSuccessEvent(observer == null ? At : observer.GetTopPosition);
			}
		};
		
		gameBoardController.RendererContext.AddAnimationToQueue(animation);
		
		return true;
	}
}