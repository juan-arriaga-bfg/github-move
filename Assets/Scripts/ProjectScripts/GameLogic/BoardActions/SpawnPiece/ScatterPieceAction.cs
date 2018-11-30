using System;
using System.Collections.Generic;
using System.Linq;

public class ScatterPieceAction : IBoardAction
{
	public static readonly int ComponentGuid = ECSManager.GetNextGuid();
	public virtual int Guid => ComponentGuid;

	public BoardPosition? From;
	public Func<BoardPosition> GetFrom;

	public bool IsTargetReplace;
	
	public Dictionary<int, int> Pieces { get; set; }
	
	public Action OnComplete { get; set; }
	
	public bool PerformAction(BoardController gameBoardController)
	{
		if (From == null) From = GetFrom?.Invoke();
		if (From == null) return false;
		
		var pieces = new Dictionary<BoardPosition, Piece>();
		var cells = new List<BoardPosition>();
		var amount = Pieces.Sum(pair => pair.Value);
		
		if (IsTargetReplace)
		{
			amount -= 1;
			
			
		}
		
		if (gameBoardController.BoardLogic.EmptyCellsFinder.FindRandomNearWithPointInCenter(From.Value, cells, amount, 0.1f) == false) return false;
		
		for (var i = 0; i < amount; i++)
		{
			var id = GetPieceId();

			if (id == PieceType.None.Id) break;

			CreatePiece(gameBoardController, id, cells[0], pieces);
			
			var cell = cells[0];
			var piece = gameBoardController.CreatePieceFromType(id);
				
			if (gameBoardController.BoardLogic.AddPieceToBoard(cell.X, cell.Y, piece) == false) continue;
				
			cells.RemoveAt(0);
			pieces.Add(cell, piece);
			gameBoardController.BoardLogic.LockCell(cell, this);

			if (cells.Count == 0) break;
		}
		
		
		
		
		
		
		
		
		
		
		
		
		
		
		
		
		
		
		
		
		

		foreach (var pair in temp)
		{
			if(cells.Count == 0) break;
			
			for (var i = 0; i < pair.Value; i++)
			{
				var cell = cells[0];
				var piece = gameBoardController.CreatePieceFromType(pair.Key);
				
				if (gameBoardController.BoardLogic.AddPieceToBoard(cell.X, cell.Y, piece) == false) continue;
				
				cells.RemoveAt(0);
				pieces.Add(cell, piece);
				gameBoardController.BoardLogic.LockCell(cell, this);
				
				if(cells.Count == 0) break;
			}
		}
		
		gameBoardController.BoardLogic.LockCell(From.Value, this);
		
		
		
		
		
		
		
		
		var animation = new ReproductionPieceAnimation
		{
			From = From.Value,
			Pieces = pieces
		};

		animation.OnCompleteEvent += (_) =>
		{
			gameBoardController.BoardLogic.UnlockCell(From.Value, this);

			foreach (var pair in pieces)
			{
				gameBoardController.BoardLogic.UnlockCell(pair.Key, this);
			}

			OnComplete?.Invoke();
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
}