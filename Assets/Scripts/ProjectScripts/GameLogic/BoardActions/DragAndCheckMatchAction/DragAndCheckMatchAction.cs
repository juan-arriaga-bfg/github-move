using System;
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

	private List<BoardPosition> targetPositions;
	private List<BoardPosition> fromPositions;
	
	public bool PerformAction(BoardController gameBoardController)
	{
		Debug.LogFormat("From: {0}", From);
		Debug.LogFormat("To: {0}", To);
		
		var pieceFrom = gameBoardController.BoardLogic.GetPieceAt(From);

//		if (pieceFrom == null)
//		{
//			Reset(gameBoardController.RendererContext);
//			return false;
//		}
		
		var multicellular =
			pieceFrom.GetComponent<MulticellularPieceBoardObserver>(MulticellularPieceBoardObserver.ComponentGuid);

		/*targetPositions = GetAllPiecePoints(pieceFrom, To);
		fromPositions = GetAllPiecePoints(pieceFrom , From);*/
		
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
		if (To.IsValidFor(board.BoardDef.Width, board.BoardDef.Height) == false)
		{
			return false;
		}

		if (targetPositions.Count == 1 && From.Equals(To))
			return false;

		var pieceFrom = board.BoardLogic.GetPieceAt(From);
		foreach (var position in targetPositions)
		{
			var pieceTo = board.BoardLogic.GetPieceAt(position);
			if (pieceFrom.Equals(pieceTo))
				continue;
			
			if (board.BoardLogic.IsLockedCell(position))
				return false;
			
			
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
			StandartPieceDrag(board);
		else
			MoveMutlicellularPieces(board);
	}

	private bool LogicMulticellularSwapPieces(BoardController board, out List<Piece> FromPieces, out List<Piece> ToPieces)
	{
		if (fromPositions.Count != targetPositions.Count)
		{
			Reset(board.RendererContext);
			FromPieces = new List<Piece>();
			ToPieces = new List<Piece>();
			return false;
		}
			
		var saveFrom = new List<Piece>();
		var saveTarget = new List<Piece>();
		
		var logic = board.BoardLogic;
		
		var countPositions = fromPositions.Count;
		
		var multicellularPiece = logic.GetPieceAt(From);
		
		for (int i = 0; i < countPositions; i++)
		{
			var fromPiece = logic.GetPieceAt(fromPositions[i]);
			var toPiece = logic.GetPieceAt(targetPositions[i]);
			
			saveFrom.Add(fromPiece);
			saveTarget.Add(toPiece == multicellularPiece ? null : toPiece);	
//			saveTarget.Add(toPiece);	
			logic.RemovePieceFromBoardSilent(fromPositions[i]);
			if(saveTarget[i] != null)
				logic.RemovePieceFromBoardSilent(targetPositions[i]);
		}

		for (int i = 0; i < countPositions; i++)
		{
			var from = fromPositions[i];
			var to = targetPositions[i];

			logic.AddPieceToBoardSilent(to.X, to.Y, saveFrom[i]);
			
			if (saveTarget[i] != null)
			{
				logic.AddPieceToBoardSilent(from.X, from.Y, saveTarget[i]);
				
				var observerTo = saveTarget[i].GetComponent<PieceBoardObserversComponent>(PieceBoardObserversComponent.ComponentGuid);
				if (observerTo != null)
				{
					observerTo.OnMovedFromTo(to, from);
				}
			}
		}
		
		var observerFrom = multicellularPiece.GetComponent<PieceBoardObserversComponent>(PieceBoardObserversComponent.ComponentGuid);
		if (observerFrom != null)
		{
			observerFrom.OnMovedFromTo(From, To);
		}

		FromPieces = saveFrom;
		ToPieces = saveTarget;

		

		return true;
	}

	private void ViewMulticellularSwapPieces(BoardController board, List<Piece> fromPieces, List<Piece> targetPieces)
	{
		var logic = board.BoardLogic;
		
		board.BoardLogic.LockCells(fromPositions, this);
		board.BoardLogic.LockCells(targetPositions, this);
		
		var animation = new MulticellularSwapPiecesAnimation()
		{
			FromPieces = fromPieces,
			TargetPieces = targetPieces,
			FromPositions = fromPositions,
			TargetPositions = targetPositions,
			To = To,
			From = From
		};

		animation.OnCompleteEvent += (_) =>
		{
			logic.UnlockCells(fromPositions, this);
			logic.UnlockCells(targetPositions, this);
		};
		
		board.RendererContext.AddAnimationToQueue(animation);
	}

	private void MulticellularSwapPieces(BoardController board)
	{
		List<Piece> fromPieces;
		List<Piece> targetPieces;
		if(LogicMulticellularSwapPieces(board, out fromPieces, out targetPieces))
			ViewMulticellularSwapPieces(board, fromPieces, targetPieces);
	}

	private void StandartPieceDrag(BoardController board)
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

	private List<BoardPosition> GetAllPiecePoints(Piece piece, BoardPosition shift)
	{
		var multicellular =
			piece.GetComponent<MulticellularPieceBoardObserver>(MulticellularPieceBoardObserver.ComponentGuid);
		if (multicellular != null)
		{
			var result = new List<BoardPosition>();
			for (int i = 0; i < multicellular.Mask.Count; i++)
			{
				result.Add(multicellular.Mask[i] + shift);
			}

			return result;
		}

		return new List<BoardPosition> {shift};
	}
	
	private void MovePiece(BoardController board, BoardPosition from, BoardPosition to)
	{
		var logic = board.BoardLogic;

		//var fromPiece = board.BoardLogic.GetPieceAt(from);
		
		//var fromPositions = GetAllPiecePoints(piece, from);
		//var targetPositions = GetAllPiecePoints(piece, to);

		logic.LockCell(from, this);
		logic.LockCell(to, this);
		
		/*for (int i = 0; i < fromPositions.Count; i++)
		{
			logic.LockCell(fromPositions[i], this);
			logic.LockCell(targetPositions[i], this);	
		}*/	

		var animation = new MovePieceFromToAnimation
		{
			From = from,
			To = to
		};

		animation.OnCompleteEvent += (_) =>
		{
			logic.UnlockCell(from, this);
			logic.UnlockCell(to, this);
			/*for (int i = 0; i < fromPositions.Count; i++)
			{
				logic.UnlockCell(fromPositions[i], this);
				logic.UnlockCell(targetPositions[i], this);	
			}*/
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

	private void MoveMutlicellularPieces(BoardController board)
	{
		var logic = board.BoardLogic;
		
		var from = new List<BoardPosition>();
		var fromPiece = logic.GetPieceAt(From);		

		var piecePositions = new List<BoardPosition>();
		for (int i = 0; i < targetPositions.Count; i++)
		{
			var pieceTo = logic.GetPieceAt(targetPositions[i]);
			if(pieceTo != null && !pieceTo.Equals(fromPiece))
				piecePositions.Add(targetPositions[i]);
		}

		var targetFreeCount = piecePositions.Count + targetPositions.Count - 1;
		var emptyFinder = logic.EmptyCellsFinder;
		
		var free = new List<BoardPosition>();
		if (!emptyFinder.FindRandomNearWithPointInCenter(To,  free, targetFreeCount*3))
		{
			MulticellularSwapPieces(board);
			return;
		}

		if (free.Count < targetFreeCount)
		{
			MulticellularSwapPieces(board);
			return;
		}
		
		from.AddRange(piecePositions);
		from.Add(From);
		
		//var freeSpace = new List<BoardPosition>();
		var current = 0;
		while (current < free.Count)
		{
			if (targetPositions.Contains(free[current]))
			{
				free.RemoveAt(current);
				continue;
			}

			current++;
		}

		var freeSpace = NormalizeFreeList(free, piecePositions);

		var to = new List<BoardPosition>();
		to.AddRange(freeSpace);
		to.Add(To);
		
		
		for (int i = 0; i < from.Count; i++)
		{
			logic.MovePieceFromTo(from[i], to[i]);	
		}
		
		logic.LockCells(from, this);
		logic.LockCells(to, this);
		
		var animation = new MovePiecesFromToAnimation
		{
			From = from,
			To = to
		};

		animation.OnCompleteEvent += (_) =>
		{
			logic.UnlockCells(from, this);
			logic.UnlockCells(to, this);
		};
		
		board.RendererContext.AddAnimationToQueue(animation);
	}

	private List<BoardPosition> NormalizeFreeList(List<BoardPosition> free, List<BoardPosition> piecesPositions)
	{
		var normalized = new List<BoardPosition>();
		foreach (var piecePos in piecesPositions)
		{
			var near = free[0];
			var value = Math.Abs(free[0].X - piecePos.X) + Math.Abs(free[0].Y - piecePos.Y);
			for (int i = 1; i < free.Count; i++)
			{
				var tmp = Math.Abs(free[i].X - piecePos.X) + Math.Abs(free[i].Y - piecePos.Y);
				if (tmp < value)
				{
					value = tmp;
					near = free[i];
				}
			}

			free.Remove(near);
			normalized.Add(near);
		}

		return normalized;
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