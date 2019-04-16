public class AirShipPieceSpawnAction : IBoardAction
{
	public static readonly int ComponentGuid = ECSManager.GetNextGuid();
	public virtual int Guid => ComponentGuid;

	public int PieceId;
	public BoardPosition At;
	public AirShipView View;
	
	public bool PerformAction(BoardController gameBoardController)
	{
		var piece = gameBoardController.CreatePieceFromType(PieceId);
		
		At = new BoardPosition(At.X, At.Y, piece.Layer.Index);

		if (gameBoardController.BoardLogic.AddPieceToBoard(At.X, At.Y, piece) == false)
		{
			return false;
		}
		
		gameBoardController.BoardLogic.LockCell(At, this);
		
		var animation = new AirShipPieceSpawnAnimation
		{
			CreatedPiece = piece,
			Action = this
		};

		animation.OnCompleteEvent += (_) =>
		{
			gameBoardController.BoardLogic.UnlockCell(At, this);
		};
		
		gameBoardController.RendererContext.AddAnimationToQueue(animation);
		
		return true;
	}
}
