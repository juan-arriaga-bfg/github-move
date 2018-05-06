public class SpawnPieceBuilder : SimplePieceBuilder
{
	public override Piece Build(int pieceType, BoardController context)
	{
		var piece = base.Build(pieceType, context);
		var def = GameDataService.Current.PiecesManager.GetPieceDefOrDefault(pieceType);
		
		CreateViewComponent(piece);
		
		piece.RegisterComponent(new TimerComponent
		{
			Delay = def.Delay
		});
        
		var storage = new StorageComponent
		{
			SpawnPiece = def.SpawnPieceType,
			Capacity = def.SpawnCapacity,
			Filling = def.IsFilledInStart ? def.SpawnCapacity : 0,
			Amount = def.SpawnAmount
		};
		
		piece.RegisterComponent(storage);
		AddObserver(piece, storage);
		
		piece.RegisterComponent(new TouchReactionComponent()
			.RegisterComponent(new TouchReactionDefinitionSpawnInStorageAndRegress())
			.RegisterComponent(new TouchReactionConditionComponent()));
		
		return piece;
	}
}