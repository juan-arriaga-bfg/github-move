public class ResourcePieceBuilder : SimplePieceBuilder
{
	public int Amount;
	public string Currency;
	
	public override Piece Build(int pieceType, BoardController context)
	{
		var piece = base.Build(pieceType, context);
		
		piece.RegisterComponent(new TouchReactionComponent()
			.RegisterComponent(new TouchReactionDefinitionCollectResource{Amount = this.Amount, CurrencyName = this.Currency})
			.RegisterComponent(new TouchReactionConditionComponent()));
		
		return piece;
	}
}