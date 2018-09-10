public class BuildingBigPieceBuilder : MulticellularPieceBuilder 
{
	public override Piece Build(int pieceType, BoardController context)
	{
		var piece = base.Build(pieceType, context);
        
		CreateViewComponent(piece);

		var state = new PieceStateComponent{StartState = BuildingState.InProgress};
		
		piece.RegisterComponent(state);
		AddObserver(piece, state);
		
		piece.RegisterComponent(new DraggablePieceComponent());
		
		piece.RegisterComponent(new TouchReactionComponent()
			.RegisterComponent(new TouchReactionDefinitionBuilding())
			.RegisterComponent(new TouchReactionConditionComponent()));
		
		return piece;
	}
}