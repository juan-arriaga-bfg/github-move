using System;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPiecesAction : IBoardAction
{
	public static readonly int ComponentGuid = ECSManager.GetNextGuid();
	
	public virtual int Guid
	{
		get { return ComponentGuid; }
	}
	
	public bool IsCheckMatch { get; set; }
	
	public BoardPosition At { get; set; }
	
	public List<int> Pieces { get; set; }

	public Action<List<BoardPosition>> OnSuccessEvent { get; set; }

	public bool PerformAction(BoardController gameBoardController)
	{
		var free = new List<BoardPosition>();
		var pieces = Pieces;
		
		pieces.Sort();
		
		if (gameBoardController.BoardLogic.EmptyCellsFinder.FindRandomNearWithPointInCenter(At, free, Pieces.Count) == false)
		{
			return false;
		}
		
		var index = free.IndexOf(At);

		free.RemoveAt(index != -1 ? index : 0);
		free.Add(At);
		
		for (int i = 0; i < pieces.Count; i++)
		{
			gameBoardController.ActionExecutor.AddAction(new SpawnPieceAtAction
			{
				IsCheckMatch = IsCheckMatch,
				At = free[i],
				PieceTypeId = pieces[i]
			});
		}

		if (OnSuccessEvent != null)
		{
			OnSuccessEvent(free);
		}
		
		return true;
	}
}