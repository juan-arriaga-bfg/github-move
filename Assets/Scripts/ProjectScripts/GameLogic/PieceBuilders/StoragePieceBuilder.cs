public class StoragePieceBuilder : MulticellularPieceBuilder
{
	public override Piece Build(int pieceType, BoardController context)
	{
		var piece = base.Build(pieceType, context);
		
		AddView(piece, ViewType.LevelLabel);
		
		piece.RegisterComponent(new TouchReactionComponent()
			.RegisterComponent(new TouchReactionDefinitionMenu{MainReactionIndex = 0}
				.RegisterDefinition(new TouchReactionDefinitionOpenWindow{WindowType = UIWindowType.StorageWindow})
				.RegisterDefinition(new TouchReactionDefinitionUpgrade {Icon = "arrow_light"}))
			.RegisterComponent(new TouchReactionConditionComponent()));
		
		return piece;
	}
}