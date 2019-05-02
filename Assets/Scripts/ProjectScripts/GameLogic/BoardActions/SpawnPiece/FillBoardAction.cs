using System;
using System.Collections.Generic;

public class FillBoardAction : IBoardAction
{
	public static readonly int ComponentGuid = ECSManager.GetNextGuid();
	public virtual int Guid => ComponentGuid;

	public int Piece { get; set; }
	public List<BoardPosition> Positions { get; set; }
	public Action<BoardController> OnComplete;
	
	public bool PerformAction(BoardController gameBoardController)
	{
		var counter = 0;
		foreach (var at in Positions)
		{
			var piece = gameBoardController.CreatePieceFromType(Piece);

			gameBoardController.BoardLogic.AddPieceToBoard(at.X, at.Y, piece);
			
			gameBoardController.RendererContext.AddAnimationToQueue(new SpawnPieceAtAnimation
			{
				CreatedPiece = piece,
				At = at,
				OnComplete = (board) =>
				{
					counter++;
					if (counter == Positions.Count)
					{
						OnComplete?.Invoke(gameBoardController);
					}
				} 
			});
		}
				
		return true;
	}
}