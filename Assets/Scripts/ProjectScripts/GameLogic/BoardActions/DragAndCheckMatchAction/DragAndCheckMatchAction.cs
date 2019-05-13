using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DragAndCheckMatchAction : IBoardAction
{
	public static readonly int ComponentGuid = ECSManager.GetNextGuid();
	public virtual int Guid => ComponentGuid;

	public BoardPosition From { get; set; }
	public BoardPosition To { get; set; }

	private List<BoardPosition> targetPositions;
	private List<BoardPosition> fromPositions;
	
	public bool PerformAction(BoardController gameBoardController)
	{
		var pieceFrom = gameBoardController.BoardLogic.GetPieceAt(From);
		
		targetPositions = new List<BoardPosition> {To};
		fromPositions = new List<BoardPosition> {From};
		
		if (pieceFrom.Multicellular != null)
		{
			targetPositions = pieceFrom.Multicellular.Mask.ToList();
			fromPositions = pieceFrom.Multicellular.Mask.ToList();
			for (int i = 0; i < pieceFrom.Multicellular.Mask.Count; i++)
			{
				var target = targetPositions[i] + To;
			    target.Z = BoardLayer.Piece.Layer;
				targetPositions[i] = target;

				var from = fromPositions[i] + From;
			    from.Z = BoardLayer.Piece.Layer;
				fromPositions[i] = from;
			}
		}
		
		if (CheckValid(gameBoardController) == false)
		{
			Reset(pieceFrom, gameBoardController.RendererContext);
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
		if (From.Equals(To)) return false;
		
		var pieceFrom = board.BoardLogic.GetPieceAt(From);
		
		return pieceFrom.Draggable != null && pieceFrom.Draggable.IsValidDrag(To);
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

	private bool LogicMulticellularSwapPieces(BoardController board, out Piece MulticellularPiece, out List<BoardPosition> allSpace, out List<Piece> allPieces)
	{
		var logic = board.BoardLogic;
		var multicellularPiece = logic.GetPieceAt(From);
		
		if (fromPositions.Count != targetPositions.Count)
		{
			Reset(multicellularPiece, board.RendererContext);
			MulticellularPiece = null;
			allPieces = new List<Piece>();
			allSpace = new List<BoardPosition>();
			return false;
		}
		
		allPieces = new List<Piece>();
		
		var saveTarget = new List<Piece>();
		var saveTargetPositions = new List<BoardPosition>();
		
		var countPositions = fromPositions.Count;

		for (int i = 0; i < countPositions; i++)
		{
			var targetPos = targetPositions[i];
			var fromPos = fromPositions[i];
			
			var toPiece = logic.GetPieceAt(targetPos);

			if (toPiece != null && multicellularPiece != toPiece)
			{
				saveTarget.Add(toPiece);
				saveTargetPositions.Add(targetPos);
			}

			logic.RemovePieceFromBoardSilent(fromPos);
			
			if (toPiece != null)
			{
				logic.RemovePieceFromBoardSilent(targetPos);
			}
		}

		allSpace = targetPositions.Concat(fromPositions).Distinct().ToList();
		for (int i = 0; i < fromPositions.Count; i++)
		{
			var to = targetPositions[i];
			allPieces.Add(multicellularPiece);
			logic.AddPieceToBoardSilent(to.X, to.Y, multicellularPiece);
		}
		
		for (int i = fromPositions.Count; i < allSpace.Count; i++)
		{
			var target = allSpace[i];
			var index = i - fromPositions.Count;

			if (index >= saveTarget.Count)
				break;

			var piece = saveTarget[index];
			var fromPiecePosition = saveTargetPositions[index];
			
			allPieces.Add(piece);
			logic.AddPieceToBoardSilent(target.X, target.Y, piece);
				
			var observerTo = saveTarget[index].GetComponent<PieceBoardObserversComponent>(PieceBoardObserversComponent.ComponentGuid);
			observerTo?.OnMovedFromToFinish(fromPiecePosition, target);
		}
		
		var observerFrom = multicellularPiece.GetComponent<PieceBoardObserversComponent>(PieceBoardObserversComponent.ComponentGuid);
		observerFrom?.OnMovedFromToFinish(From, To);

		MulticellularPiece = multicellularPiece;

		return true;
	}

	private void ViewMulticellularSwapPieces(BoardController board, Piece multicellular, List<BoardPosition> allSpace, List<Piece> allPieces)
	{
		var logic = board.BoardLogic;
	    
	    multicellular?.ViewDefinition?.OnSwap(false);
	    for (int i = 0; i < allPieces.Count; i++)
	    {
	        var allPiece = allPieces[i];
	        allPiece?.ViewDefinition?.OnSwap(false);
	    }

		board.BoardLogic.LockCells(fromPositions, this);
		board.BoardLogic.LockCells(targetPositions, this);
		
		var animation = new MulticellularSwapPiecesAnimation
		{
			MulticellularPiece = multicellular,
			AllSpace = allSpace,
			AllPieces = allPieces,
			FromPositions = fromPositions,
			To = To
		};

		animation.OnCompleteEvent += (_) =>
		{
			logic.UnlockCells(fromPositions, this);
			logic.UnlockCells(targetPositions, this);
		    
		    multicellular?.ViewDefinition?.OnSwap(true);
		    for (int i = 0; i < allPieces.Count; i++)
		    {
		        var allPiece = allPieces[i];
		        allPiece?.ViewDefinition?.OnSwap(true);
		    }
		};
		
		board.RendererContext.AddAnimationToQueue(animation);
	}

	private void MulticellularSwapPieces(BoardController board)
	{
		Piece multicellularPiece;
		List<Piece> allPieces;
		List<BoardPosition> allSpace;
		if(LogicMulticellularSwapPieces(board, out multicellularPiece, out allSpace, out allPieces))
			ViewMulticellularSwapPieces(board, multicellularPiece, allSpace, allPieces);
	}

	private void StandartPieceDrag(BoardController board)
	{
		var logic = board.BoardLogic;
		
		var pieceFrom = logic.GetPieceAt(From);
		var pieceTo = logic.GetPieceAt(To);

		if (pieceFrom.PieceType == PieceType.Boost_CR.Id || pieceFrom.PieceType == pieceTo.PieceType)
		{
			if (pieceFrom.Matchable?.IsMatchable() == true && CheckMatch(board, new List<BoardPosition> {From}, out var action))
			{
				board.ActionExecutor.PerformAction(action);
				board.BoardLogic.FireflyLogic.OnMatch();
				return;
			}
			
			MoveCheckAndAnimation(board);
			return;
		}
		
		if (CheckSwapLogic(board, out var free))
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

		action = null;
		
		if (logic.FieldFinder.Find(To, matchField, out var currentId) == false) return false;
		
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
		if (CheckSwapLogic(board, out var free))
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
	    
	    var pieceFrom = logic.GetPieceAt(From);
	    var pieceTo = logic.GetPieceAt(To);
		
		logic.LockCell(From, this);
		logic.LockCell(To, this);
	    
	    pieceFrom?.ViewDefinition?.OnSwap(false);
	    pieceTo?.ViewDefinition?.OnSwap(false);
	    
		var animation = new SwapPiecesAnimation
		{
			PointA = From,
			PointB = To
		};

		animation.OnCompleteEvent += (_) =>
		{
			logic.UnlockCell(From, this);
			logic.UnlockCell(To, this);
		    
		    pieceFrom?.ViewDefinition?.OnSwap(true);
		    pieceTo?.ViewDefinition?.OnSwap(true);
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
		if (!emptyFinder.FindRandomNearWithPointInCenter(To,  free, targetFreeCount, 0.1f))
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
			if (free.Count == 0)
			{
				break;
			}
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
	
	private void Reset(Piece context, BoardRenderer renderer)
	{
		var abortAnimation = new ResetPiecePositionAnimation
		{
			At = From,
			OnCompleteEvent = animation =>
			{
				context.ViewDefinition?.OnDrag(true);
				context.GetComponent<PartPieceBoardObserver>(PartPieceBoardObserver.ComponentGuid)?.AddBubble(context.CachedPosition, context.PieceType);
			}
		};
		
		renderer.AddAnimationToQueue(abortAnimation);
	}
}