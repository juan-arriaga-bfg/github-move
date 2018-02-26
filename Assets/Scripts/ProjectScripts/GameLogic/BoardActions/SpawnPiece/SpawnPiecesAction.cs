using System.Collections.Generic;

public class SpawnPiecesAction : IBoardAction
{
	public static readonly int ComponentGuid = ECSManager.GetNextGuid();
	
	public virtual int Guid
	{
		get { return ComponentGuid; }
	}
	
	public BoardPosition At { get; set; }
	
	public List<int> Pieces { get; set; }
	
	public bool PerformAction(BoardController gameBoardController)
	{
		var free = new List<BoardPosition>();
		var pieces = Pieces;
		
		pieces.Sort();
		gameBoardController.BoardLogic.EmptyCellsFinder.FindNearWithPointInCenter(At, free, Pieces.Count, 5);

		var index = free.IndexOf(At);

		if (index != -1)
		{
			free.RemoveAt(index);
			free.Add(At);
		}

		for (int i = 0; i < free.Count; i++)
		{
			gameBoardController.ActionExecutor.AddAction(new SpawnPieceAtAction
			{
				IsCheckMatch = true,
				At = free[i],
				PieceTypeId = pieces[i]
			});
		}
		
		return true;
	}
}