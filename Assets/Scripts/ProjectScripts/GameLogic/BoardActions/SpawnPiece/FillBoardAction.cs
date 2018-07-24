using System.Collections.Generic;

public class FillBoardAction : IBoardAction
{
	public static readonly int ComponentGuid = ECSManager.GetNextGuid();
	
	public virtual int Guid
	{
		get { return ComponentGuid; }
	}

	public int Piece { get; set; }
	public List<BoardPosition> Positions { get; set; }
	
	public bool PerformAction(BoardController gameBoardController)
	{
		for (int i = 0; i < Positions.Count; i++)
		{
			gameBoardController.ActionExecutor.AddAction(new SpawnPieceAtAction
			{
				IsCheckMatch = false,
				At = Positions[i],
				PieceTypeId = Piece
			});
		}
		
		return true;
	}
}