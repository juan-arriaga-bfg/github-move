public class UpgradePieceBuilder : MulticellularPieceBuilder
{
	public override Piece Build(int pieceType, BoardController context)
	{
		var piece = base.Build(pieceType, context);
		
		piece.RegisterComponent(new TouchReactionComponent()
			.RegisterComponent(new TouchReactionDefinitionUpgrade())
			.RegisterComponent(new TouchReactionConditionComponent()));
        
		return piece;
	}
}