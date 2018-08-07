using System.Collections.Generic;

public class FillBoardAction : IBoardAction
{
	public static readonly int ComponentGuid = ECSManager.GetNextGuid();
	public virtual int Guid => ComponentGuid;

	public int Piece { get; set; }
	public List<BoardPosition> Positions { get; set; }
	
	public bool PerformAction(BoardController gameBoardController)
	{
		foreach (var at in Positions)
		{
			gameBoardController.ActionExecutor.AddAction(new SpawnPieceAtAction
			{
				IsCheckMatch = false,
				At = at,
				PieceTypeId = Piece
			});
		}

		return true;
	}
}