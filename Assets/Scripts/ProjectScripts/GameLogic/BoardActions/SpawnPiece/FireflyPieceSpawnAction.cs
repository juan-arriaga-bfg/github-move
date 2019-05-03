public class FireflyPieceSpawnAction : IBoardAction
{
	public static readonly int ComponentGuid = ECSManager.GetNextGuid();
	public virtual int Guid => ComponentGuid;

	public int PieceId;
	public BoardPosition At;
	public FireflyView View;
	public FireflyType FireflyType = FireflyType.Production;
	
	public bool PerformAction(BoardController gameBoardController)
	{
		var piece = gameBoardController.CreatePieceFromType(PieceId);
		
		At = new BoardPosition(At.X, At.Y, piece.Layer.Index);

		if (gameBoardController.BoardLogic.AddPieceToBoard(At.X, At.Y, piece) == false)
		{
			return false;
		}
		
		gameBoardController.BoardLogic.LockCell(At, this);
		
		var animation = new FireflyPieceSpawnAnimation
		{
			CreatedPiece = piece,
			Action = this,
			FireflyType = FireflyType
		};

		animation.OnCompleteEvent += (_) =>
		{
			gameBoardController.BoardLogic.UnlockCell(At, this);
		};
		
		gameBoardController.RendererContext.AddAnimationToQueue(animation);
		
		return true;
	}
}
