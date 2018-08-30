public class MovePieceFromToAction : IBoardAction
{
	public static readonly int ComponentGuid = ECSManager.GetNextGuid();
	public virtual int Guid => ComponentGuid;

	public BoardPosition From { get; set; }
	
	public BoardPosition To { get; set; }
	
	public bool PerformAction(BoardController gameBoardController)
	{
		To = new BoardPosition(To.X, To.Y, From.Z);
		
		bool state = (gameBoardController.BoardLogic.IsLockedCell(To) == false)
		             && (gameBoardController.BoardLogic.IsEmpty(To))
		             && (gameBoardController.BoardLogic.MovePieceFromTo(From, To));

		if (state == false)
		{
			var abortAnimation = new ResetPiecePositionAnimation
			{
				At = From
			};
			
			gameBoardController.RendererContext.AddAnimationToQueue(abortAnimation);
			
			return false;
		}
		
		gameBoardController.BoardLogic.LockCell(From, this);
		gameBoardController.BoardLogic.LockCell(To, this);
		
		var animation = new MovePieceFromToAnimation
		{
			From = From,
			To = To
		};

		animation.OnCompleteEvent += (_) =>
		{
			gameBoardController.BoardLogic.UnlockCell(From, this);
			gameBoardController.BoardLogic.UnlockCell(To, this);
			
			gameBoardController.ActionExecutor.AddAction(new CheckMatchAction
			{
				At = To
			});
		};
		
		gameBoardController.RendererContext.AddAnimationToQueue(animation);
		
		return true;
	}
}

