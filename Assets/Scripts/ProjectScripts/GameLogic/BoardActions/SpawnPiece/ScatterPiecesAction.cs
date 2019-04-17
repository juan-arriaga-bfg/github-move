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
	public bool IsSingle;
	public bool IsSetRewardToComplete = true;

	public Dictionary<int, int> Pieces;
	private Dictionary<int, int> fakePieces;

	public Action<bool> OnComplete;

	public bool RewardEffect = false;

	public bool PerformAction(BoardController gameBoardController)
	{
		fakePieces = new Dictionary<int, int>(Pieces);
		
		var target = gameBoardController.BoardLogic.GetPieceAt(From);
		var rewardsStore = target.GetComponent<RewardsStoreComponent>(RewardsStoreComponent.ComponentGuid);
		
		var next = new Dictionary<BoardPosition, Piece>();
		var pieces = new Dictionary<BoardPosition, Piece>();
		
		var cells = new List<BoardPosition>();
		
		var amount = fakePieces.Sum(pair => pair.Value);

		if (IsTargetReplace) amount = Mathf.Max(0, amount - (target.Multicellular?.Mask.Count ?? 1));

		var fakeAmount = IsSingle && amount > 0 ? 1 : amount;
		
		if (fakeAmount > 0 && gameBoardController.BoardLogic.EmptyCellsFinder.FindRandomNearWithPointInCenter(From, cells, fakeAmount, 0.1f) == false)
		{
			rewardsStore?.ShowBubble();
			return false;
		}

		var animation = new ScatterPiecesAnimation
		{
			From = From, 
			AnimationResourceSearchOnRemove = piece => AnimationOverrideDataService.Current.FindAnimation(piece, def => def.OnDestroyFromBoard),
			RewardEffect = RewardEffect
		};
		
		if (IsTargetReplace && cells.Count >= amount)
		{
			var mask = new List<BoardPosition>();
			
			if (target.Multicellular != null)
			{
				foreach (var cell in target.Multicellular.Mask)
				{
					mask.Add(target.Multicellular.GetPointInMask(target.CachedPosition, cell));
				}
			}
			else
			{
				mask.Add(From);
			}
			
			foreach (var cell in mask)
			{
				var id = GetPieceId(true);
				
				if (id == PieceType.None.Id) break;
			
				CreatePiece(gameBoardController, id, cell, next);
			}
			
			animation.Replace = next;
		}
		
		gameBoardController.BoardLogic.LockCell(From, this);

		foreach (var cell in cells)
		{
			var id = GetPieceId(false);
			
			if (id == PieceType.None.Id) break;
			
			CreatePiece(gameBoardController, id, cell, pieces);
		}

		if (IsSetRewardToComplete)
		{
			rewardsStore.IsComplete = true;
		}

		animation.Pieces = pieces;
		animation.OnCompleteEvent += (_) =>
		{
			var legacy = new Dictionary<int, int>(Pieces);
			
			foreach (var pair in legacy)
			{
				if (fakePieces.TryGetValue(pair.Key, out var value) == false)
				{
					Pieces.Remove(pair.Key);
					continue;
				}

				Pieces[pair.Key] = value;
			}
			
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
				
				rewardsStore.IsComplete = false;
				return;
			}
			
			rewardsStore?.ShowBubble();
		};
		
		gameBoardController.RendererContext.AddAnimationToQueue(animation);
		
		return true;
	}
	
	private int GetPieceId(bool isReplace)
	{
		var id = PieceType.None.Id;

		if (fakePieces.Count == 0) return id;
		
		foreach (var key in fakePieces.Keys)
		{
			if(isReplace == false && PieceType.GetDefById(key).Filter.HasFlag(PieceTypeFilter.Obstacle)) continue;
			
			id = key;
			break;
		}

		var value = fakePieces[id] - 1;
		
		if (value == 0) fakePieces.Remove(id);
		else fakePieces[id] = value;

		return id;
	}

	private void CreatePiece(BoardController board, int id, BoardPosition position, Dictionary<BoardPosition, Piece> pieces)
	{
		var piece = board.CreatePieceFromType(id);
		
		pieces.Add(position, piece);
		board.BoardLogic.LockCell(position, this);
	}
}