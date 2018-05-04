public class SimpleObstaclePieceBuilder : GenericPieceBuilder
{
	public override Piece Build(int pieceType, BoardController context)
	{
		var piece = base.Build(pieceType, context);

		piece.RegisterComponent(new ObstacleLifeComponent());
		
		piece.RegisterComponent(new TouchReactionComponent()
			.RegisterComponent(new TouchReactionDefinitionSimpleObstacle())
			.RegisterComponent(new TouchReactionConditionComponent()));

		CreateViewComponent(piece);
		
		return piece;
	}
}