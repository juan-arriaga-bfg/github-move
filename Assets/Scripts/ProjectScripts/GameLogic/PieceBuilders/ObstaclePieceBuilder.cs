public class ObstaclePieceBuilder : GenericPieceBuilder
{
	public override Piece Build(int pieceType, BoardController context)
	{
		var piece = base.Build(pieceType, context);

		var life = new ObstacleLifeComponent();

		piece.RegisterComponent(life);
		AddObserver(piece, life);
		
		piece.RegisterComponent(new TouchReactionComponent()
			.RegisterComponent(new TouchReactionDefinitionObstacle())
			.RegisterComponent(new TouchReactionConditionComponent()));

		CreateViewComponent(piece);
		
		return piece;
	}
}