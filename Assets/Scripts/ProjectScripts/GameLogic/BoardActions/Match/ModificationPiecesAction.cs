using Debug = IW.Logger;
using System;
using System.Collections;
using System.Collections.Generic;
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
		
		if (gameBoardController.BoardLogic.EmptyCellsFinder.FindRandomNearWithPointInCenter(To, free, NextPieces.Count) == false) return false;
		
		var index = free.IndexOf(To);

		free.RemoveAt(index != -1 ? index : 0);
		free.Add(To);
		NextPieces.Sort();
		
		var pieces = new List<Piece>();
		
		for (var i = 0; i < NextPieces.Count; i++)
		{
			var piece = gameBoardController.CreatePieceFromType(NextPieces[i]);
			var position = free[i];

			if (gameBoardController.BoardLogic.AddPieceToBoard(position.X, position.Y, piece) == false)
			{
				Debug.LogErrorFormat("Can't create piece with id {0} at {1}", NextPieces[i], position);
				free.Remove(position);
				continue;
			}
			
			pieces.Add(piece);
		}
		
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
			Positions = Positions
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