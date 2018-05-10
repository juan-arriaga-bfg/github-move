using System.Collections.Generic;

public class DragAndCheckMatchAction : IBoardAction
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
		if (CheckValid(gameBoardController) == false)
		{
			Reset(gameBoardController.RendererContext);
			return false;
		}

		if (CheckFreeCell(gameBoardController))
		{
			return true;
		}

		CheckCurrentType(gameBoardController);
		return true;
	}
	
	private bool CheckValid(BoardController board)
	{
		if (To.IsValidFor(board.BoardDef.Width, board.BoardDef.Height) == false
		    || board.BoardLogic.IsLockedCell(To)
		    || From.Equals(To))
		{
			return false;
		}
		
		var pieceTo = board.BoardLogic.GetPieceAt(To);

		if (pieceTo == null) return true;
		
		var draggable = pieceTo.GetComponent<DraggablePieceComponent>(DraggablePieceComponent.ComponentGuid);
		
		return draggable != null && draggable.IsDraggable(To);
	}
	
	private bool CheckFreeCell(BoardController board)
	{
		var logic = board.BoardLogic;
		
		if (logic.IsEmpty(To) == false)
		{
			return false;
		}
		
		logic.MovePieceFromTo(From, To);
		
		/*IBoardAction action;
		
		if (CheckMatch(board, null, out action))
		{
			board.RendererContext.MoveElement(From, To);
			board.ActionExecutor.PerformAction(action);
			return true;
		}*/
		
		MovePiece(board, From, To);
		return true;
	}
	
	private void CheckCurrentType(BoardController board)
	{
		var logic = board.BoardLogic;
		
		var pieceFrom = logic.GetPieceAt(From);
		var pieceTo = logic.GetPieceAt(To);
		
		IBoardAction action;

		if (pieceFrom.PieceType == pieceTo.PieceType)
		{
			if (CheckMatch(board, new List<BoardPosition> {From}, out action))
			{
				board.ActionExecutor.PerformAction(action);
				return;
			}
			
			MoveCheckAndAnimation(board);
			return;
		}
		
		BoardPosition free;
		var isSwap = CheckSwapLogic(board, out free);

		/*if (CheckMatch(board, null, out action))
		{
			board.RendererContext.SwapElements(From, To);
			board.ActionExecutor.PerformAction(action);
			MovePiece(board, From, isSwap ? From : free);
			return;
		}*/
		
		if (isSwap)
		{
			SwapPieces(board);
			return;
		}
		
		MovePieces(board, free);
	}
	
	private bool CheckMatch(BoardController board, List<BoardPosition> matchField, out IBoardAction action)
	{
		matchField = matchField ?? new List<BoardPosition>();
		
		var logic = board.BoardLogic;
		
		int currentId;

		action = null;
		
		if (logic.FieldFinder.Find(To, matchField, out currentId) == false) return false;
		
		action = logic.MatchActionBuilder.GetMatchAction(matchField, currentId, To);

		return action != null;
	}

	private bool CheckSwapLogic(BoardController board, out BoardPosition free)
	{
		var logic = board.BoardLogic;
		free = BoardPosition.Default();

		if (To.IsNeighbor(From) == false)
		{
			var points = new List<BoardPosition>();

			if (logic.EmptyCellsFinder.FindRandomNearWithPointInCenter(To, points, 1))
			{
				free = points[0];

				logic.MovePieceFromTo(To, free);
				logic.MovePieceFromTo(From, To);

				return false;
			}
		}
		
		logic.SwapPieces(From, To);
		return true;
	}
	
	private void MoveCheckAndAnimation(BoardController board)
	{
		BoardPosition free;

		if (CheckSwapLogic(board, out free))
		{
			SwapPieces(board);
			return;
		}
		
		MovePieces(board, free);
	}
	
	private void MovePiece(BoardController board, BoardPosition from, BoardPosition to)
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
		};

		board.RendererContext.AddAnimationToQueue(animation);
	}

	private void SwapPieces(BoardController board)
	{
		var logic = board.BoardLogic;
		
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
		};
		
		board.RendererContext.AddAnimationToQueue(animation);
	}

	private void MovePieces(BoardController board, BoardPosition free)
	{
		var logic = board.BoardLogic;
		
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
		};
		
		board.RendererContext.AddAnimationToQueue(animation);
	}
	
	private void Reset(BoardRenderer renderer)
	{
		var abortAnimation = new ResetPiecePositionAnimation
		{
			At = From
		};
		
		renderer.AddAnimationToQueue(abortAnimation);
	}
}