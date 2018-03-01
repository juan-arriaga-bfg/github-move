public class ResourcePieceBuilder : SimplePieceBuilder
{
	public override Piece Build(int pieceType, BoardController context)
	{
		var piece = base.Build(pieceType, context);

		piece.RegisterComponent(new ResourceStorageComponent());
		
		piece.RegisterComponent(new TouchReactionComponent()
			.RegisterComponent(new TouchReactionDefinitionCollectResource())
			.RegisterComponent(new TouchReactionConditionComponent()));
		
		return piece;
	}
}