using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ChestRewardAction : IBoardAction
{
	public static readonly int ComponentGuid = ECSManager.GetNextGuid();

	public virtual int Guid
	{
		get { return ComponentGuid; }
	}
	
	public BoardPosition From { get; set; }
	public List<CurrencyPair> Rewards;
	public IBoardAction OnComplete;

	public bool PerformAction(BoardController gameBoardController)
	{
		var pieces = new Dictionary<BoardPosition, Piece>();
		var free = new List<BoardPosition>();
		var count = Rewards.Sum(pair => pair.Amount);
		
		if (gameBoardController.BoardLogic.EmptyCellsFinder.FindRandomNearWithPointInCenter(From, free, count, 5) == false)
		{
			return false;
		}

		foreach (var reward in Rewards)
		{
			if(free.Count == 0) break;

			var pieceType = PieceType.Parse(reward.Currency);
			
			for (var i = 0; i < reward.Amount; i++)
			{
				if(free.Count == 0) break;
				
				var pos = free[0];
				var piece = gameBoardController.CreatePieceFromType(pieceType);
				
				free.RemoveAt(0);

				if (pos.IsValid == false 
				    || gameBoardController.BoardLogic.IsLockedCell(pos)
				    || gameBoardController.BoardLogic.AddPieceToBoard(pos.X, pos.Y, piece) == false)
				{
					continue;
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
			
			if(OnComplete != null) gameBoardController.ActionExecutor.AddAction(OnComplete);
		};
		
		gameBoardController.RendererContext.AddAnimationToQueue(animation);

		return true;
	}
}