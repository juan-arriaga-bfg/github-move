using System.Collections.Generic;

public class MoveFromToAndCheckCoverMatchAction : IBoardAction
{
	public static readonly int ComponentGuid = ECSManager.GetNextGuid();

	public virtual int Guid
	{
		get { return ComponentGuid; }
	}

	public BoardPosition From { get; set; }
	
	public BoardPosition To { get; set; }
	
	public bool PerformAction(BoardController gameBoardController)
	{
		var logic = gameBoardController.BoardLogic;
		
		To = new BoardPosition(To.X, To.Y, From.Z);
		
		var pieceFrom = logic.GetPieceAt(From);
		int pieceTo;
		var matchField = new List<BoardPosition>();
		
		logic.FieldFinder.Find(To, matchField, out pieceTo);

		matchField.Remove(From);
		
		if (logic.IsLockedCell(To) || (pieceTo != PieceType.None.Id && pieceFrom.PieceType != pieceTo) || matchField.Count == 1)
		{
			var abortAnimation = new ResetPiecePositionAnimation
			{
				At = From
			};
			
			gameBoardController.RendererContext.AddAnimationToQueue(abortAnimation);
			
			return false;
		}
		
		matchField = null;

		if (pieceTo == PieceType.None.Id)
		{
			logic.MovePieceFromTo(From, To);
		}
		else
		{
			matchField = new List<BoardPosition> {From};
		}
		
		logic.LockCell(From, this);
		logic.LockCell(To, this);
		
		var animation = new MovePieceFromToAnimation
		{
			From = From,
			To = To
		};
		
		animation.OnCompleteEvent += (_) =>
		{
			logic.UnlockCell(From, this);
			logic.UnlockCell(To, this);
			
			gameBoardController.ActionExecutor.AddAction(new CheckMatchAction
			{
				At = To,
				MatchField = matchField
			});
		};
		
		gameBoardController.RendererContext.AddAnimationToQueue(animation);
		
		return true;
	}
}