public class BuildingBigPieceBuilder : MulticellularPieceBuilder
{
	public BuildingState StartState = BuildingState.InProgress;
	
	public override Piece Build(int pieceType, BoardController context)
	{
		var piece = base.Build(pieceType, context);
        
		CreateViewComponent(piece);

		AddObserver(piece, new PieceStateComponent {StartState = StartState});
		
		piece.RegisterComponent(new DraggablePieceComponent());
		
		piece.RegisterComponent(new TouchReactionComponent()
			.RegisterComponent(new TouchReactionDefinitionBuilding())
			.RegisterComponent(new TouchReactionConditionComponent()));
		
		return piece;
	}
}