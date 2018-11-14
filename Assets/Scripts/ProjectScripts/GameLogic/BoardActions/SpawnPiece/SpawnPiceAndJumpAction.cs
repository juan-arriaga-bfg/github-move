public class SpawnPiceAndJumpAction : IBoardAction
{
	public static readonly int ComponentGuid = ECSManager.GetNextGuid();
	public virtual int Guid
	{
		get { return ComponentGuid; }
	}
	
	public int Piece { get; set; }
	
	public BoardPosition From { get; set; }
	public BoardPosition To { get; set; }

	public bool PerformAction(BoardController gameBoardController)
	{
		var piece = gameBoardController.CreatePieceFromType(Piece);

		if (To.IsValid == false
		    || gameBoardController.BoardLogic.IsLockedCell(To)
		    || gameBoardController.BoardLogic.AddPieceToBoard(To.X, To.Y, piece) == false)
		{
			return false;
		}
		
		gameBoardController.BoardLogic.LockCell(To, this);
		
		var animation = new SpawnPiceAndJumpAnimation
		{
			Piece = piece,
			From = From,
			To = To
		};
		
		animation.OnCompleteEvent += (_) =>
		{
			gameBoardController.BoardLogic.UnlockCell(To, this);
//			gameBoardController.PathfindLocker.OnAddComplete();
		};
		
		gameBoardController.RendererContext.AddAnimationToQueue(animation);
		return true;
	}
}