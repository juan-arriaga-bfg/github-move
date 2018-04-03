public class SimpleObstaclePieceBuilder : GenericPieceBuilder
{
	public override Piece Build(int pieceType, BoardController context)
	{
		var piece = base.Build(pieceType, context);
		
		piece.RegisterComponent(new TouchReactionComponent()
			.RegisterComponent(new TouchReactionDefinitionSimpleObstacle())
			.RegisterComponent(new TouchReactionConditionComponent()));
		
		AddView(piece, ViewType.SimpleObstacle);
		
		return piece;
	}
}