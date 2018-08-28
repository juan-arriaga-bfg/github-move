using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ChestRewardAction : IBoardAction
{
	public static readonly int ComponentGuid = ECSManager.GetNextGuid();
	public virtual int Guid => ComponentGuid;

	public BoardPosition From { get; set; }
	public Dictionary<int, int> Pieces = new Dictionary<int, int>();
	public IBoardAction OnComplete;
	public Action OnCompleteAction;
	
	public bool PerformAction(BoardController gameBoardController)
	{
		var pieces = new Dictionary<BoardPosition, Piece>();
		var free = new List<BoardPosition>();
		var count = Pieces.Sum(pair => pair.Value);
		
		if (Pieces.Count != 0 && gameBoardController.BoardLogic.EmptyCellsFinder.FindRandomNearWithPointInCenter(From, free, count, 0.1f) == false)
		{
			UIErrorWindowController.AddError("Not found free cells");
			return false;
		}

		var piecesKeys = Pieces.Keys.ToList();
		foreach (var rewardKey in piecesKeys)
		{
			if(free.Count == 0) break;

			var pieceType = rewardKey;
			
			while(Pieces[rewardKey] > 0)
			{
				if(free.Count == 0) break;
				
				Pieces[rewardKey]--;
				
				var pos = free[0];
				var piece = gameBoardController.CreatePieceFromType(pieceType);
				
				free.RemoveAt(0);

				if (gameBoardController.BoardLogic.AddPieceToBoard(pos.X, pos.Y, piece) == false)
				{
					break;
				}
				
				pieces.Add(pos, piece);
				gameBoardController.BoardLogic.LockCell(pos, this);
			}
		}

		
		gameBoardController.BoardLogic.LockCell(From, this);
		
		var animation = new ReproductionPieceAnimation
		{
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

			OnCompleteAction?.Invoke();
			if (OnComplete != null) gameBoardController.ActionExecutor.AddAction(OnComplete);
		};
		
		gameBoardController.RendererContext.AddAnimationToQueue(animation);
		
		return true;
	}
}