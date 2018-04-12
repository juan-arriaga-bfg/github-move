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
		var pieces = new Dictionary<BoardPosition, Piece>();
		
		foreach (var pos in Positions)
		{
			var piece = gameBoardController.CreatePieceFromType(Piece);

			if (pos.IsValid == false
			    || gameBoardController.BoardLogic.IsLockedCell(pos)
			    || gameBoardController.BoardLogic.AddPieceToBoard(pos.X, pos.Y, piece) == false)
			{
				continue;
			}
			
			pieces.Add(pos, piece);
			gameBoardController.BoardLogic.LockCell(pos, this);
		}
		
		gameBoardController.BoardLogic.LockCell(From, this);
		
		var animation = new ReproductionPieceAnimation
		{
			From = From,
			Pieces = pieces
		};

		animation.OnCompleteEvent += (_) =>
		{
			gameBoardController.BoardLogic.UnlockCell(From, this);

			foreach (var pair in pieces)
			{
				gameBoardController.BoardLogic.UnlockCell(pair.Key, this);
			}
		};
		
		gameBoardController.RendererContext.AddAnimationToQueue(animation);

		return true;
	}
}