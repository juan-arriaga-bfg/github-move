public class ProductionPieceBuilder : MulticellularPieceBuilder
{
	public override Piece Build(int pieceType, BoardController context)
	{
		var piece = base.Build(pieceType, context);
		
		AddView(piece, ViewType.LevelLabel);
		
		var upgrade = new UpgradeComponent();
		var production = new ProductionComponent();
		
		piece.RegisterComponent(upgrade);
		AddObserver(piece, upgrade);
		
		piece.RegisterComponent(production);
		AddObserver(piece, production);
		
		piece.RegisterComponent(new TouchReactionComponent()
			.RegisterComponent(new TouchReactionDefinitionMenu{MainReactionIndex = 0}
				.RegisterDefinition(new TouchReactionDefinitionProduction{Production = production})
				.RegisterDefinition(new TouchReactionDefinitionUpgrade {Icon = "arrow_light"}))
			.RegisterComponent(new TouchReactionConditionComponent()));
		
		return piece;
	}
}