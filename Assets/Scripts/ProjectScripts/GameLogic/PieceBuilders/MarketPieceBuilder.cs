public class MarketPieceBuilder : MulticellularPieceBuilder
{
	public override Piece Build(int pieceType, BoardController context)
	{
		var piece = base.Build(pieceType, context);
		
		AddView(piece, ViewType.LevelLabel);
		
		var upgrade = new UpgradeComponent();
		
		piece.RegisterComponent(upgrade);
		AddObserver(piece, upgrade);
		
		piece.RegisterComponent(new TouchReactionComponent()
			.RegisterComponent(new TouchReactionDefinitionMenu{MainReactionIndex = 0}
				.RegisterDefinition(new TouchReactionDefinitionOpenWindow{WindowType = UIWindowType.MarketWindow})
				.RegisterDefinition(new TouchReactionDefinitionUpgrade {Icon = "arrow_light"}))
			.RegisterComponent(new TouchReactionConditionComponent()));
		
		return piece;
	}
}