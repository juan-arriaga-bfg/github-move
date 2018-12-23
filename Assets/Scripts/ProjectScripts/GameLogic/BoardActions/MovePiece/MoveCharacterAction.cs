public class MoveCharacterAction : IBoardAction
{
	public static readonly int ComponentGuid = ECSManager.GetNextGuid();
	public virtual int Guid => ComponentGuid;

	public BoardPosition From;
	
	public bool PerformAction(BoardController gameBoardController)
	{
		var logic = gameBoardController.BoardLogic;
		var positions = From.NeighborsFor(gameBoardController.BoardDef.Width, gameBoardController.BoardDef.Height);
		BoardPosition? to = null;
		
		positions.Shuffle();

		foreach (var position in positions)
		{
			if (logic.IsLockedCell(position)
			    || logic.IsEmpty(position) == false
			    || logic.MovePieceFromTo(From, position) == false) continue;

			to = position;
			break;
		}

		if (to == null) return false;
		
		gameBoardController.BoardLogic.LockCell(From, this);
		gameBoardController.BoardLogic.LockCell(to.Value, this);
		
		var animation = new MovePieceFromToAnimation
		{
			From = From,
			To = to.Value
		};

		animation.OnCompleteEvent += (_) =>
		{
			gameBoardController.BoardLogic.UnlockCell(From, this);
			gameBoardController.BoardLogic.UnlockCell(to.Value, this);
		};
		
		gameBoardController.RendererContext.AddAnimationToQueue(animation);
		
		return true;
	}
}