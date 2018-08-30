public class CharacterPieceBuilder : GenericPieceBuilder
{
	public override Piece Build(int pieceType, BoardController context)
	{
		var piece = base.Build(pieceType, context);
		var def = GameDataService.Current.PiecesManager.GetPieceDef(pieceType);
		
		CreateViewComponent(piece);
		
		piece.RegisterComponent(new TimerComponent{Delay = 10});
		
		var storage = new StorageComponent
		{
			SpawnPiece = PieceType.Parse(def.SpawnResources.Currency),
			IsAutoStart = false,
			IsTimerShow = false,
			Amount = def.SpawnResources.Amount,
			Capacity = def.SpawnResources.Amount
		};
		
		piece.RegisterComponent(storage);
		AddObserver(piece, storage);
		
		piece.RegisterComponent(new TouchReactionComponent()
			.RegisterComponent(new TouchReactionDefinitionMenu {MainReactionIndex = 0}
				.RegisterDefinition(new TouchReactionDefinitionOpenWindow{WindowType = UIWindowType.QuestWindow})
				.RegisterDefinition(new TouchReactionDefinitionSpawnInStorage {IsAutoStart = false})
				.RegisterDefinition(new TouchReactionDefinitionSpawnShop()))
			.RegisterComponent(new TouchReactionConditionComponent()));
		
		return piece;
	}
}