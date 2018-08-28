public class QuestGiverPieceBuilder : GenericPieceBuilder
{
	public override Piece Build(int pieceType, BoardController context)
	{
		var piece = base.Build(pieceType, context);
		
		piece.RegisterComponent(new TouchReactionComponent()
			.RegisterComponent(new TouchReactionDefinitionOpenWindow{WindowType = UIWindowType.QuestWindow})
			.RegisterComponent(new TouchReactionConditionComponent()));
		
		return piece;
	}
}