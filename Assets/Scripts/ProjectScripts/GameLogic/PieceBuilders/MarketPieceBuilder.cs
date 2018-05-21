﻿public class MarketPieceBuilder : MulticellularPieceBuilder
{
	public override Piece Build(int pieceType, BoardController context)
	{
		var piece = base.Build(pieceType, context);
		
		/*piece.RegisterComponent(new TouchReactionComponent()
			.RegisterComponent(new TouchReactionDefinitionOpenWindow{Icon = "face_Robin", WindowType = UIWindowType.StorageWindow})
			.RegisterComponent(new TouchReactionConditionComponent()));*/
		
		return piece;
	}
}