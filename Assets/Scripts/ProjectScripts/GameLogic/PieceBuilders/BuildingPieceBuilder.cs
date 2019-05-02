public class BuildingPieceBuilder : GenericPieceBuilder 
{
	public override Piece Build(int pieceType, BoardController context)
	{
		var piece = base.Build(pieceType, context);
        
		CreateViewComponent(piece);
		
		AddObserver(piece, new PieceStateComponent());
		
		piece.RegisterComponent(new DraggablePieceComponent());
		
		piece.RegisterComponent(new TouchReactionComponent()
			.RegisterComponent(new TouchReactionDefinitionBuilding())
			.RegisterComponent(new TouchReactionConditionComponent()));
		
		return piece;
	}
}