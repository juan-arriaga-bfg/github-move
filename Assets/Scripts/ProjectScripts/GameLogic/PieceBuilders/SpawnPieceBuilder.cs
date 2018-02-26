public class SpawnPieceBuilder : IPieceBuilder
{
	public int SpawnPieceType;
	public int Delay;
	
	public Piece Build(int pieceType, BoardController context)
	{
		var piece = new Piece(pieceType, context);

		piece.RegisterComponent(new LayerPieceComponent {Index = context.BoardDef.PieceLayer});
		piece.RegisterComponent(new DraggablePieceComponent());
		piece.RegisterComponent(new PieceBoardObserversComponent());

		piece.RegisterComponent(new TouchReactionComponent()
			.RegisterComponent(new TouchReactionDefinitionSpawnPieceComponent{SpawnPieceType = SpawnPieceType})
			.RegisterComponent(new TouchReactonConditionDelayComponent{Delay = Delay}));

		return piece;
	}
}