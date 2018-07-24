public class QuestGiverPieceBuilder : GenericPieceBuilder
{
	public override Piece Build(int pieceType, BoardController context)
	{
		var piece = base.Build(pieceType, context);
		
		piece.RegisterComponent(new TouchReactionComponent()
			.RegisterComponent(new TouchReactionDefinitionOpenQuestWindow())
			.RegisterComponent(new TouchReactionConditionComponent()));
		
		return piece;
	}
}