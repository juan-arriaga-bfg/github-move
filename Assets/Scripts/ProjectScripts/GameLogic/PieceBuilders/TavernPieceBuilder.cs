public class TavernPieceBuilder : MulticellularPieceBuilder
{
	public override Piece Build(int pieceType, BoardController context)
	{
		var piece = base.Build(pieceType, context);
		
		piece.RegisterComponent(new TouchReactionComponent()
			.RegisterComponent(new TouchReactionDefinitionMenu()
				.RegisterDefinition(new TouchReactionDefinitionOpenCharacterWindow(), "face_Robin")
				.RegisterDefinition(new TouchReactionDefinitionUpgrade(), "arrow"))
			.RegisterComponent(new TouchReactionConditionComponent()));
        
		var observer = piece.GetComponent<PieceBoardObserversComponent>(PieceBoardObserversComponent.ComponentGuid);

		if (observer != null)
		{
			observer.RegisterObserver(new HeroObserver());
		}
		
		return piece;
	}
}