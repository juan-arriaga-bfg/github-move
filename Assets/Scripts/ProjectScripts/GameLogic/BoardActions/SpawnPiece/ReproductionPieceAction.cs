using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class ReproductionPieceAction : IBoardAction
{
	public static readonly int ComponentGuid = ECSManager.GetNextGuid();

	public virtual int Guid
	{
		get { return ComponentGuid; }
	}
	
	public BoardElementView BoardElement;
	public BoardPosition From { get; set; }
	public int Piece { get; set; }
	public List<BoardPosition> Positions { get; set; }
	public Action OnComplete;

	public bool PerformAction(BoardController gameBoardController)
	{
		var pieces = new Dictionary<BoardPosition, Piece>();

		List<BoardPosition> cellsForLock = new List<BoardPosition>();
		
		foreach (var pos in Positions)
		{
			var piece = gameBoardController.CreatePieceFromType(Piece);

			if (gameBoardController.BoardLogic.AddPieceToBoard(pos.X, pos.Y, piece) == false)
			{
				continue;
			}
			
			pieces.Add(pos, piece);
			cellsForLock.Add(pos);
		}

		gameBoardController.BoardLogic.LockCells(cellsForLock, this);
		gameBoardController.BoardLogic.LockCell(From, this);
		
		var animation = new ReproductionPieceAnimation
		{
			BoardElement = BoardElement,
			From = From,
			Pieces = pieces
		};

		animation.OnCompleteEvent += (_) =>
		{
			gameBoardController.BoardLogic.UnlockCell(From, this);

			foreach (var pair in pieces)
			{
				gameBoardController.BoardLogic.UnlockCell(pair.Key, this);
			}

			if (OnComplete != null) OnComplete();
		};
		
		gameBoardController.RendererContext.AddAnimationToQueue(animation);

		return true;
	}
}