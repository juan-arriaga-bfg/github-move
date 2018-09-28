using UnityEngine;

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
			SpawnPiece = Currency.GetCurrencyDef(def.SpawnResources.Currency).Id,
			IsAutoStart = false,
			IsTimerShow = false,
			Amount = def.SpawnResources.Amount,
			Capacity = def.SpawnResources.Amount,
			BubbleOffset = new Vector3(-0.1f, 2f)
		};
		
		piece.RegisterComponent(storage);
		AddObserver(piece, storage);
		
		var pathfindLockObserver = new PathfindLockObserver() {AutoLock = true}; 
		AddObserver(piece, pathfindLockObserver);
		piece.RegisterComponent(pathfindLockObserver);
		
		piece.RegisterComponent(new DraggablePieceComponent());
		
		piece.RegisterComponent(new TouchReactionComponent()
			.RegisterComponent(new TouchReactionDefinitionMenu {MainReactionIndex = 0}
				.RegisterDefinition(new TouchReactionDefinitionOpenWindow{WindowType = UIWindowType.CastleWindow})
				.RegisterDefinition(new TouchReactionDefinitionSpawnInStorage {IsAutoStart = false})
				.RegisterDefinition(new TouchReactionDefinitionSpawnShop()))
			.RegisterComponent(new TouchReactionConditionComponent()))
			.RegisterComponent(new PiecePathfindBoardCondition(context, piece)
				.RegisterComponent(PathfindIgnoreBuilder.Build(piece.PieceType)));
		
		return piece;
	}
}