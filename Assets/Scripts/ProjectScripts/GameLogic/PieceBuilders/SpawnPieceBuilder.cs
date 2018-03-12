public class SpawnPieceBuilder : SimplePieceBuilder
{
	public override Piece Build(int pieceType, BoardController context)
	{
		var piece = base.Build(pieceType, context);
		var def = GameDataService.Current.PiecesManager.GetPieceDefOrDefault(pieceType);
		
		piece.RegisterComponent(new TouchReactionComponent()
			.RegisterComponent(new TouchReactionDefinitionSpawnPiece())
			.RegisterComponent(new TouchReactonConditionDelay{Delay = def.Delay}));

		return piece;
	}
}