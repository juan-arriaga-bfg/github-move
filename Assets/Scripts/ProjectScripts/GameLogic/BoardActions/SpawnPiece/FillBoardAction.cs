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
			var piece = gameBoardController.CreatePieceFromType(Piece);
			
			gameBoardController.BoardLogic.AddPieceToBoard(at.X, at.Y, piece);
			
			gameBoardController.RendererContext.AddAnimationToQueue(new SpawnPieceAtAnimation
			{
				CreatedPiece = piece,
				At = at
			});
		}
		
		return true;
	}
}