public class SimpleObstaclePieceBuilder : GenericPieceBuilder
{
	public override Piece Build(int pieceType, BoardController context)
	{
		var piece = base.Build(pieceType, context);
		
		piece.RegisterComponent(new TouchReactionComponent()
			.RegisterComponent(new TouchReactionDefinitionSimpleObstacle())
			.RegisterComponent(new TouchReactionConditionComponent()));

		var isMultiple = context.BoardLogic.MatchDefinition.GetPrevious(pieceType) != PieceType.None.Id;
		
		AddView(piece, isMultiple ? ViewType.SimpleMultipleObstacle : ViewType.SimpleObstacle);
		
		return piece;
	}
}