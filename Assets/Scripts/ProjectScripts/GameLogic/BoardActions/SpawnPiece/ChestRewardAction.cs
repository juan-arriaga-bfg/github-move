﻿using System.Collections.Generic;
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
	public Dictionary<int, int> Pieces = new Dictionary<int, int>();
	public Dictionary<string, int> Chargers = new Dictionary<string, int>();
	public IBoardAction OnComplete;

	public bool IsAddCollection = true;

	public bool PerformAction(BoardController gameBoardController)
	{
		var pieces = new Dictionary<BoardPosition, Piece>();
		var free = new List<BoardPosition>();
		var count = Pieces.Sum(pair => pair.Value);
		
		if (Pieces.Count != 0 && gameBoardController.BoardLogic.EmptyCellsFinder.FindRandomNearWithPointInCenter(From, free, count) == false)
		{
			return false;
		}
		
		foreach (var reward in Chargers)
		{
			for (var i = 0; i < reward.Value; i++)
			{
				GameDataService.Current.CollectionManager.CastResourceOnBoard(
					new BoardPosition(From.X, From.Y, From.Z + i + 1),
					new CurrencyPair{Currency = reward.Key, Amount = 1});
			}
		}

		if (IsAddCollection)
		{
			var collection = GameDataService.Current.CollectionManager.GetRandomItem();

			if (string.IsNullOrEmpty(collection) == false)
			{
				GameDataService.Current.CollectionManager.CastResourceOnBoard(From,
					new CurrencyPair{Currency = collection, Amount = 1});
			}
		}
		
		foreach (var reward in Pieces)
		{
			if(free.Count == 0) break;

			var pieceType = reward.Key;
			
			for (var i = 0; i < reward.Value; i++)
			{
				if(free.Count == 0) break;
				
				var pos = free[0];
				var piece = gameBoardController.CreatePieceFromType(pieceType);
				
				free.RemoveAt(0);

				if (gameBoardController.BoardLogic.AddPieceToBoard(pos.X, pos.Y, piece) == false)
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