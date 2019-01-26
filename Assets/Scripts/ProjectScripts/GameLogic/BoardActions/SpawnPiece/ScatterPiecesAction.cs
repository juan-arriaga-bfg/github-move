using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ScatterPiecesAction : IBoardAction
{
	public static readonly int ComponentGuid = ECSManager.GetNextGuid();
	public virtual int Guid => ComponentGuid;

	public BoardPosition From;

	public bool IsTargetReplace;

	public Dictionary<int, int> Pieces;

	public Action<bool> OnComplete;
	
	public bool PerformAction(BoardController gameBoardController)
	{
		var target = gameBoardController.BoardLogic.GetPieceAt(From);
		
		var next = new Dictionary<BoardPosition, Piece>();
		var pieces = new Dictionary<BoardPosition, Piece>();
		
		var mask = new List<BoardPosition>();
		var cells = new List<BoardPosition>();
		
		var amount = Pieces.Sum(pair => pair.Value);
		var replaceAmount = target.Multicellular?.Mask.Count ?? 1;

		if (IsTargetReplace) amount = Mathf.Max(0, amount - replaceAmount);

		if (amount > 0 && gameBoardController.BoardLogic.EmptyCellsFinder.FindRandomNearWithPointInCenter(From, cells, amount, 0.1f) == false)
		{
			target.GetComponent<RewardsStoreComponent>(RewardsStoreComponent.ComponentGuid)?.ShowBubble();
			return false;
		}

		var animation = new ScatterPiecesAnimation {From = From};
		
		if (IsTargetReplace && cells.Count == amount)
		{
			gameBoardController.BoardLogic.RemovePieceAt(From);
			
			if (target.Multicellular != null)
			{
				foreach (var cell in target.Multicellular.Mask)
				{
					var point = target.Multicellular.GetPointInMask(target.CachedPosition, cell);
					
					mask.Add(point);
				}
			}
			else
			{
				mask.Add(From);
			}
			
			foreach (var cell in mask)
			{
				var id = GetPieceId();
				
				if (id == PieceType.None.Id) break;
			
				CreatePiece(gameBoardController, id, cell, next);
			}
			
			animation.Replace = next;
		}
		
		gameBoardController.BoardLogic.LockCell(From, this);

		foreach (var cell in cells)
		{
			var id = GetPieceId();
			
			if (id == PieceType.None.Id) break;
			
			CreatePiece(gameBoardController, id, cell, pieces);
		}

		var rewardsStore = target.GetComponent<RewardsStoreComponent>(RewardsStoreComponent.ComponentGuid);
		
		if (rewardsStore != null) rewardsStore.IsComplete = Pieces.Count != 0;
		
		animation.Pieces = pieces;
		animation.OnCompleteEvent += (_) =>
		{
			gameBoardController.BoardLogic.UnlockCell(From, this);
			
			foreach (var pair in next)
			{
				gameBoardController.BoardLogic.UnlockCell(pair.Key, this);
			}
			
			foreach (var pair in pieces)
			{
				gameBoardController.BoardLogic.UnlockCell(pair.Key, this);
			}
			
			OnComplete?.Invoke(Pieces.Count == 0);

			if (Pieces.Count == 0)
			{
				if (IsTargetReplace && PieceType.GetDefById(target.PieceType).Filter.HasFlag(PieceTypeFilter.Obstacle)) target.PathfindLockObserver.RemoveRecalculate(From);
				return;
			}
			
			rewardsStore?.ShowBubble();
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