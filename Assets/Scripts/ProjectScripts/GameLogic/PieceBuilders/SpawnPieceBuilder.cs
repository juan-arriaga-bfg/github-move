public class SpawnPieceBuilder : SimplePieceBuilder
{
	public override Piece Build(int pieceType, BoardController context)
	{
		var piece = base.Build(pieceType, context);
		var def = GameDataService.Current.PiecesManager.GetPieceDefOrDefault(pieceType);
		
		piece.RegisterComponent(new TimerComponent
		{
			Delay = def.Delay
		});
        
		piece.RegisterComponent(new StorageComponent
		{
			SpawnPiece = def.SpawnPieceType,
			Capacity = def.SpawnCapacity,
			Filling = def.IsFilledInStart ? def.SpawnCapacity : 0
		});
		
		piece.RegisterComponent(new TouchReactionComponent()
			.RegisterComponent(new TouchReactionDefinitionSpawnInStorageAndRegress())
			.RegisterComponent(new TouchReactionConditionComponent()));
		
		AddView(piece, ViewType.StorageState);
		
		return piece;
	}
}