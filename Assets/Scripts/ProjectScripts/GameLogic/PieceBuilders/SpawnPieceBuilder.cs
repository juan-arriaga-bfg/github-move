public class SpawnPieceBuilder : SimplePieceBuilder
{
	public int SpawnPieceType;
	public int Delay;
	
	public override Piece Build(int pieceType, BoardController context)
	{
		var piece = base.Build(pieceType, context);
		
		piece.RegisterComponent(new TouchReactionComponent()
			.RegisterComponent(new TouchReactionDefinitionSpawnPiece{SpawnPieceType = this.SpawnPieceType})
			.RegisterComponent(new TouchReactonConditionDelay{Delay = this.Delay}));

		return piece;
	}
}