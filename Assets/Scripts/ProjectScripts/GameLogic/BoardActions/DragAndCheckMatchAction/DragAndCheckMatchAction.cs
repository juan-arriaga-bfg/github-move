using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DragAndCheckMatchAction : IBoardAction
{
	public static readonly int ComponentGuid = ECSManager.GetNextGuid();

	public virtual int Guid
	{
		get { return ComponentGuid; }
	}

	public BoardPosition From { get; set; }
	public BoardPosition To { get; set; }

	private IList<BoardPosition> targetPositions;
	private IList<BoardPosition> fromPositions;
	
	public bool PerformAction(BoardController gameBoardController)
	{
		Debug.LogFormat("From: {0}", From);
		Debug.LogFormat("To: {0}", To);
		
		var pieceFrom = gameBoardController.BoardLogic.GetPieceAt(From);
		var multicellular =
			pieceFrom.GetComponent<MulticellularPieceBoardObserver>(MulticellularPieceBoardObserver.ComponentGuid);
		
		targetPositions = new List<BoardPosition> {To};
		fromPositions = new List<BoardPosition> {From};
		if (multicellular != null)
		{
			targetPositions = multicellular.Mask.ToList();
			fromPositions = multicellular.Mask.ToList();
			for (int i = 0; i < multicellular.Mask.Count; i++)
			{
				targetPositions[i] += To;
				fromPositions[i] += From;
			}
		}
		
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
		    || From.Equals(To))
		{
			return false;
		}

		foreach (var position in targetPositions)
		{
				if (board.BoardLogic.IsLockedCell(position))
				return false;
		}
		
		

		foreach (var position in targetPositions)
		{
			var pieceTo = board.BoardLogic.GetPieceAt(position);
			if (pieceTo != null)
			{
				var draggable = pieceTo.GetComponent<DraggablePieceComponent>(DraggablePieceComponent.ComponentGuid);
				if (draggable == null || !draggable.IsDraggable(position) || IsLargeObject(pieceTo))
					return false;
			}
		}

		return true;
	}

	private bool IsLargeObject(Piece piece)
	{
		var multicellular =
			piece.GetComponent<MulticellularPieceBoardObserver>(MulticellularPieceBoardObserver.ComponentGuid);
		if (multicellular == null)
			return false;

		return multicellular.Mask.Count > 1;
	}
	
	private bool CheckFreeCell(BoardController board)
	{
		var logic = board.BoardLogic;

		foreach (var pos in targetPositions)
		{
			if (logic.IsEmpty(pos) == false)
			{
				return false;
			}	
		}
		
		logic.MovePieceFromTo(From, To);
		
		MovePiece(board, From, To);
		return true;
	}
	
	private void CheckCurrentType(BoardController board)
	{
		if (fromPositions.Count == 1)
		{
			Match(board);
			Swap(board);
		}
		else
		{
			
		}
	}

	private void Swap(BoardController board)
	{
		BoardPosition free;
		var isSwap = CheckSwapLogic(board, out free);
		
		if (isSwap)
		{
			SwapPieces(board);
			return;
		}
		
		MovePieces(board, free);
	}

	private void Match(BoardController board)
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
	}
	
	private bool CheckMatch(BoardController board, List<BoardPosition> matchField, out IBoardAction action)
	{
		matchField = matchField ?? new List<BoardPosition>();
		
		var logic = board.BoardLogic;
		
		int currentId;

		action = null;
		
		if (logic.FieldFinder.Find(To, matchField, out currentId) == false) return false;
		
		action = logic.MatchActionBuilder.GetMatchAction(matchField, currentId, To);

		var isMatch = action != null;

		if (isMatch)
		{
			board.ReproductionLogic.Restart();
		}

		return isMatch;
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