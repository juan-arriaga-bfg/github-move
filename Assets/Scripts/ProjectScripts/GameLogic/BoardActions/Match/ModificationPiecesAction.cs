using Debug = IW.Logger;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

public class ModificationPiecesAction : IBoardAction
{
	public static readonly int ComponentGuid = ECSManager.GetNextGuid();
	public virtual int Guid => ComponentGuid;
	
	public List<int> NextPieces;
	
	public BoardPosition To;
	public List<BoardPosition> Positions;
	
	public Action OnComplete;
	public Action<List<BoardPosition>> OnSuccess;

	public bool PerformAction(BoardController gameBoardController)
	{
		if (Positions == null || Positions.Count == 0 || Positions.Count < NextPieces.Count) return false;
		
		gameBoardController.BoardLogic.RemovePiecesAt(Positions);
		
		var free = new List<BoardPosition>();
		
		if (gameBoardController.BoardLogic.EmptyCellsFinder.FindRandomNearWithPointInCenter(To, free, NextPieces.Count * 2) == false) return false;
		
		var index = free.IndexOf(To);
		
		free.RemoveAt(index != -1 ? index : 0);
		
		// sort by distance to center
		free = free.OrderBy(x => BoardPosition.SqrMagnitude(x, To)).ToList();
		free.Insert(NextPieces.Count - 1, To);
		
		NextPieces.Sort();
		
		var pieces = new List<Piece>();
		
		for (var i = 0; i < NextPieces.Count; i++)
		{
			var piece = gameBoardController.CreatePieceFromType(NextPieces[i]);
			
			if (free.Count <= i || i < 0) break;
			
			var position = free[i];

			if (gameBoardController.BoardLogic.AddPieceToBoard(position.X, position.Y, piece) == false)
			{
				Debug.LogWarning($"Safe operation: Can't create piece with id {NextPieces[i]} at {position}");
				free.Remove(position);
				i--;
				
				continue;
			}
			
			pieces.Add(piece);
		}
		
		free.RemoveRange(NextPieces.Count, free.Count - NextPieces.Count);
		
		gameBoardController.BoardLogic.LockCells(Positions, this);
		gameBoardController.BoardLogic.LockCells(free, this);
		
		var animationSpawn = new MatchSpawnPiecesAtAnimation
		{
			Positions = free,
			Pieces = pieces
		};
		
		animationSpawn.OnCompleteEvent += (_) =>
		{
			gameBoardController.BoardLogic.UnlockCells(Positions, this);
			gameBoardController.BoardLogic.UnlockCells(free, this);
			
			OnComplete?.Invoke();
		};
		
		var animationMatch = new MatchPieceToAnimation
		{
			To = To,
			Positions = Positions,
		    NextPieces = pieces
		};
		
		animationMatch.OnCompleteEvent += (_) =>
		{
			gameBoardController.ActionExecutor.AddAction(new AnimationStartAction
			{
				Animation = animationSpawn
			});
			
			var result = new List<BoardPosition>();

			foreach (var piece in pieces)
			{
				result.Add(piece.Multicellular?.GetTopPosition ?? piece.CachedPosition);
			}
			
			OnSuccess?.Invoke(result);
		};
		
		gameBoardController.RendererContext.AddAnimationToQueue(animationMatch);
		
		return true;
	}
}