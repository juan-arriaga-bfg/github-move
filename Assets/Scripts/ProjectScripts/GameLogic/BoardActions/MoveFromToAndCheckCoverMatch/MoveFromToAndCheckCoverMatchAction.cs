using System;
using System.Collections.Generic;
using UnityEngine;

public class MoveFromToAndCheckCoverMatchAction : IBoardAction
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
		
		if (To.IsValidFor(gameBoardController.BoardDef.Width, gameBoardController.BoardDef.Height) == false
		    || From.Equals(To)
			|| CheckFreeCell(gameBoardController) == false
		    && CheckCurrentPieceType(gameBoardController) == false
		    && CheckMove(gameBoardController) == false)
		{
			Reset(gameBoardController.RendererContext);
			return false;
		}
		
		return true;
	}

	private bool CheckFreeCell(BoardController board)
	{
		var logic = board.BoardLogic;
		
		if (logic.IsLockedCell(To) || logic.IsEmpty(To) == false)
		{
			return false;
		}
		
		logic.MovePieceFromTo(From, To);
		MovePiece(board, From, To, () => CheckMatch(board, null));
		return true;
	}

	private bool CheckCurrentPieceType(BoardController board)
	{
		var logic = board.BoardLogic;
		
		var pieceFrom = logic.GetPieceAt(From);
		var pieceTo = logic.GetPieceAt(To);
		
		if (pieceFrom.PieceType != pieceTo.PieceType || pieceFrom.Matchable.IsMatchable() == false || pieceTo.Matchable.IsMatchable() == false)
		{
			return false;
		}
		
		int pieceType;
		var matchField = new List<BoardPosition>();
		
		logic.FieldFinder.Find(To, matchField, out pieceType);
		matchField.Remove(From);
		
		if (matchField.Count == 1)
		{
			return false;
		}
		
		MovePiece(board, From, To, () => CheckMatch(board, new List<BoardPosition> {From}));
		return true;
	}

	private bool CheckMove(BoardController board)
	{
		var pieceTo = board.BoardLogic.GetPieceAt(To);
		var draggable = pieceTo.GetComponent<DraggablePieceComponent>(DraggablePieceComponent.ComponentGuid);

		if (draggable == null || draggable.IsDraggable(To) == false)
		{
			return false;
		}

		if (SwapPieces(board))
		{
			return true;
		}
		
		return MovePieces(board);
	}

	private void MovePiece(BoardController board, BoardPosition from, BoardPosition to, Action onComplete)
	{
		var logic = board.BoardLogic;
		
		logic.LockCell(from, this);
		logic.LockCell(to, this);

		var animation = new MovePieceFromToAnimation
		{
			From = from,
			To = to
		};

		animation.OnCompleteEvent += (_) =>
		{
			logic.UnlockCell(from, this);
			logic.UnlockCell(to, this);

			if (onComplete != null) onComplete();
		};

		board.RendererContext.AddAnimationToQueue(animation);
	}

	private bool SwapPieces(BoardController board)
	{
		if (To.IsNeighbor(From) == false)
		{
			return false;
		}
		
		var logic = board.BoardLogic;
		
		logic.SwapPieces(From, To);
		
		logic.LockCell(From, this);
		logic.LockCell(To, this);

		var animation = new SwapPiecesAnimation
		{
			PointA = From,
			PointB = To
		};

		animation.OnCompleteEvent += (_) =>
		{
			logic.UnlockCell(From, this);
			logic.UnlockCell(To, this);
			
			CheckMatch(board, null);
		};

		board.RendererContext.AddAnimationToQueue(animation);
		return true;
	}

	private bool MovePieces(BoardController board)
	{
		var logic = board.BoardLogic;
		
		var points = new List<BoardPosition>();
			
		if(logic.EmptyCellsFinder.FindRandomNearWithPointInCenter(To, points, 1) == false)
		{
			return false;
		}
		
		var free = points[0];
		
		logic.MovePieceFromTo(To, free);
		logic.MovePieceFromTo(From, To);
		
		logic.LockCell(From, this);
		logic.LockCell(To, this);
		logic.LockCell(free, this);

		var animation = new MovePiecesFromToAnimation
		{
			From = new List<BoardPosition> { To, From },
			To = new List<BoardPosition> { free, To }
		};

		animation.OnCompleteEvent += (_) =>
		{
			logic.UnlockCell(From, this);
			logic.UnlockCell(To, this);
			logic.UnlockCell(free, this);
			
			CheckMatch(board, null);
		};

		board.RendererContext.AddAnimationToQueue(animation);
		return true;
	}

	private void Reset(BoardRenderer renderer)
	{
		var abortAnimation = new ResetPiecePositionAnimation
		{
			At = From
		};
		
		renderer.AddAnimationToQueue(abortAnimation);
	}

	private void CheckMatch(BoardController board, List<BoardPosition> matchList)
	{
		board.ActionExecutor.AddAction(new CheckMatchAction
		{
			At = To,
			MatchField = matchList
		});
	}
}