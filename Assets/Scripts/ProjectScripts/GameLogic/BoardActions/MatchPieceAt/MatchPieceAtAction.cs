using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchPieceAtAction : IBoardAction
{
	public static readonly int ComponentGuid = ECSManager.GetNextGuid();
	
	public virtual int Guid
	{
		get { return ComponentGuid; }
	}
	
	public BoardPosition At { get; set; }

	public List<BoardPosition> MatchField { get; private set; }
	
	public bool PerformAction(BoardController gameBoardController)
	{
		if (gameBoardController.BoardLogic.IsLockedCell(At)) return false;
		
		var piece = gameBoardController.BoardLogic.GetPieceAt(At);
		
		if (piece == null || piece.PieceType == PieceType.None.Id) return false;
		
		var next = gameBoardController.BoardLogic.MatchDefinition.GetNext(piece.PieceType);
		
		if(next == PieceType.None.Id) return false;

		MatchField = FindField(gameBoardController, piece.PieceType, At, new List<BoardPosition>());

		if (MatchField.Count < 3) return false;
		
		gameBoardController.BoardLogic.LockCells(MatchField, this);

		gameBoardController.BoardLogic.RemovePiecesAt(MatchField);
		
		var animation = new MatchPieceAtAnimation
		{
			Action = this
		};

		animation.OnCompleteEvent += (_) =>
		{
			gameBoardController.BoardLogic.UnlockCells(MatchField, this);
			
			gameBoardController.ActionExecutor.AddAction(new SpawnPieceAtAction
			{
				At = At,
				PieceTypeId = next
			});
		};
		
		gameBoardController.RendererContext.AddAnimationToQueue(animation);
		
		return true;
	}

	private List<BoardPosition> FindField(BoardController gameBoardController, int type, BoardPosition point, List<BoardPosition> field)
	{
		if(field.Contains(point) || gameBoardController.BoardLogic.IsLockedCell(point)) return field;
		
		field.Add(point);
		
		if (PieceIsCorrect(gameBoardController, type, point.Left))
		{
			FindField(gameBoardController, type, point.Left, field);
		}
		
		if (PieceIsCorrect(gameBoardController, type, point.Right))
		{
			FindField(gameBoardController, type, point.Right, field);
		}
		
		if (PieceIsCorrect(gameBoardController, type, point.Up))
		{
			FindField(gameBoardController, type, point.Up, field);
		}
		
		if (PieceIsCorrect(gameBoardController, type, point.Down))
		{
			FindField(gameBoardController, type, point.Down, field);
		}
		
		return field;
	}

	private bool PieceIsCorrect(BoardController gameBoardController, int type, BoardPosition point)
	{
		var piece = gameBoardController.BoardLogic.GetPieceAt(point);
		
		return piece != null && piece.PieceType == type;
	}
}