public class ProductionPieceBuilder : MulticellularPieceBuilder
{
	public override Piece Build(int pieceType, BoardController context)
	{
		var piece = base.Build(pieceType, context);
		
		CreateViewComponent(piece);
		
		var production = new ProductionComponent();
		
		piece.RegisterComponent(production);
		AddObserver(piece, production);
		
		piece.RegisterComponent(new TouchReactionComponent()
			.RegisterComponent(new TouchReactionDefinitionProduction{Production = production})
			.RegisterComponent(new TouchReactionConditionComponent()));
		
		return piece;
	}
}