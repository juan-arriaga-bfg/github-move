public class ResetPiecePositionAnimation : BoardAnimation 
{
	public BoardPosition At { get; set; }

	public override void Animate(BoardRenderer context)
	{
		var pieceFromView = context.GetElementAt(At);

		context.ResetBoardElement(pieceFromView, At);

		CompleteAnimation(context);

	}
}