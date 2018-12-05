using System;
using System.Collections.Generic;
using System.Linq;

public class ScatterPiecesAction : IBoardAction
{
	public static readonly int ComponentGuid = ECSManager.GetNextGuid();
	public virtual int Guid => ComponentGuid;

	public BoardPosition? From;
	public Func<BoardPosition> GetFrom;

	public bool IsTargetReplace;

	public Dictionary<int, int> Pieces;

	public Action OnComplete;
	
	public bool PerformAction(BoardController gameBoardController)
	{
		if (From == null)
		{
			From = GetFrom?.Invoke();
			
			if (From == null) return false;
		}
		
		var from = From.Value;
		var pieces = new Dictionary<BoardPosition, Piece>();
		var cells = new List<BoardPosition>();
		var amount = Pieces.Sum(pair => pair.Value);

		if (IsTargetReplace) amount -= 1;

		if (amount > 0 && gameBoardController.BoardLogic.EmptyCellsFinder.FindRandomNearWithPointInCenter(from, cells, amount, 0.1f) == false)
		{
			OutOfCells(gameBoardController, from);
			return false;
		}

		var animation = new ScatterPiecesAnimation {From = from};
		
		if (IsTargetReplace && cells.Count == amount)
		{
			var id = GetPieceId();
			var piece = gameBoardController.CreatePieceFromType(id);

			gameBoardController.BoardLogic.RemovePieceAt(from);
			gameBoardController.BoardLogic.AddPieceToBoard(from.X, from.Y, piece);

			animation.Replace = piece;
		}
		
		gameBoardController.BoardLogic.LockCell(from, this);

		foreach (var cell in cells)
		{
			var id = GetPieceId();
			
			if (id == PieceType.None.Id) break;
			
			CreatePiece(gameBoardController, id, cell, pieces);
		}
		
		animation.Pieces = pieces;
		animation.OnCompleteEvent += (_) =>
		{
			gameBoardController.BoardLogic.UnlockCell(from, this);

			foreach (var pair in pieces)
			{
				gameBoardController.BoardLogic.UnlockCell(pair.Key, this);
			}
			
			OnComplete?.Invoke();
			
			if(Pieces.Count == 0) return;

			OutOfCells(gameBoardController, from);
		};
		
		gameBoardController.RendererContext.AddAnimationToQueue(animation);

		return true;
	}

	private int GetPieceId()
	{
		var id = PieceType.None.Id;

		if (Pieces.Count == 0) return id;
		
		foreach (var key in Pieces.Keys)
		{
			id = key;
			break;
		}

		var value = Pieces[id] - 1;
		
		if (value == 0) Pieces.Remove(id);
		else Pieces[id] = value;

		return id;
	}

	private void CreatePiece(BoardController board, int id, BoardPosition position, Dictionary<BoardPosition, Piece> pieces)
	{
		var piece = board.CreatePieceFromType(id);
				
		if (board.BoardLogic.AddPieceToBoard(position.X, position.Y, piece) == false) return;
		
		pieces.Add(position, piece);
		board.BoardLogic.LockCell(position, this);
	}

	private void OutOfCells(BoardController board, BoardPosition position)
	{
		var rewards = board.BoardLogic.GetPieceAt(position)?.GetComponent<RewardsStoreComponent>(RewardsStoreComponent.ComponentGuid);

		rewards?.ShowBubble();
	}
}