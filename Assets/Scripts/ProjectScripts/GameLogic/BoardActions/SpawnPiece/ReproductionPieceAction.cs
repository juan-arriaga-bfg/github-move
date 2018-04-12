using System.Collections.Generic;

public class ReproductionPieceAction : IBoardAction
{
	public static readonly int ComponentGuid = ECSManager.GetNextGuid();

	public virtual int Guid
	{
		get { return ComponentGuid; }
	}
	
	public BoardPosition From { get; set; }
	public int Piece { get; set; }
	public List<BoardPosition> Positions { get; set; }

	public bool PerformAction(BoardController gameBoardController)
	{
		var pieces = new List<Piece>();
		
		for (var i = Positions.Count - 1; i >= 0; i--)
		{
			var pos = Positions[i];
			var piece = gameBoardController.CreatePieceFromType(Piece);
			
			if (pos.IsValid
			    && gameBoardController.BoardLogic.IsLockedCell(pos) == false
			    && gameBoardController.BoardLogic.AddPieceToBoard(pos.X, pos.Y, piece))
			{
				pieces.Add(piece);
				continue;
			}
			
			Positions.Remove(pos);
		}
		
		gameBoardController.BoardLogic.LockCell(From, this);
		gameBoardController.BoardLogic.LockCells(Positions, this);
		
		var animation = new ReproductionPieceAnimation
		{
			Action = this,
			Pieces = pieces
		};

		animation.OnCompleteEvent += (_) =>
		{
			gameBoardController.BoardLogic.UnlockCell(From, this);
			gameBoardController.BoardLogic.UnlockCells(Positions, this);
		};
		
		gameBoardController.RendererContext.AddAnimationToQueue(animation);

		return true;
	}
}