using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using Newtonsoft.Json;

[JsonObject(MemberSerialization.OptIn)]
public class FieldDefComponent : ECSEntity, IECSSerializeable 
{
	public static int ComponentGuid = ECSManager.GetNextGuid();

	public override int Guid { get { return ComponentGuid; } }

	private List<PieceSaveItem> pieces;
	private List<ChestSaveItem> chests;
	private List<LifeSaveItem> lifes;
	private List<StorageSaveItem> storages;
	private List<ResourceSaveItem> resources;
	private List<ProductionSaveItem> productions;
	
	private string completeFog;

	[JsonProperty]
	public List<PieceSaveItem> Pieces
	{
		get { return pieces; }
		set { pieces = value; }
	}
	
	[JsonProperty]
	public List<ChestSaveItem> Chests
	{
		get { return chests; }
		set { chests = value; }
	}
	
	[JsonProperty]
	public List<LifeSaveItem> Lifes
	{
		get { return lifes; }
		set { lifes = value; }
	}
	
	[JsonProperty]
	public List<StorageSaveItem> Storages
	{
		get { return storages; }
		set { storages = value; }
	}
	
	[JsonProperty]
	public List<ResourceSaveItem> Resources
	{
		get { return resources; }
		set { resources = value; }
	}
	
	[JsonProperty]
	public string CompleteFog
	{
		get { return completeFog; }
		set { completeFog = value; }
	}
	
//	[JsonProperty]
	public List<ProductionSaveItem> Productions
	{
		get { return productions; }
		set { productions = value; }
	}

	public List<BoardPosition> CompleteFogPositions;
	
	private Dictionary<BoardPosition, StorageSaveItem> storageSave;
	private Dictionary<BoardPosition, LifeSaveItem> lifeSave;
	
	[OnSerializing]
	internal void OnSerialization(StreamingContext context)
	{
		if(BoardService.Current == null) return;
		
		var board = BoardService.Current.GetBoardById(0);
		var cash = board.BoardLogic.PositionsCache.Cache;
		var match = board.BoardLogic.MatchDefinition;
		
		if(cash.Count == 0) return;
		
		pieces = new List<PieceSaveItem>();
		chests = new List<ChestSaveItem>();
		lifes = new List<LifeSaveItem>();
		storages = new List<StorageSaveItem>();
		productions = new List<ProductionSaveItem>();
		
		resources = GetResourceSave();
		
		foreach (var item in cash)
		{
			if(item.Value.Count == 0) continue;
			
			pieces.Add(GetPieceSave(item.Key, item.Value));

			var id = match.GetFirst(item.Key);
			
			storages.AddRange(GetStorageSave(board.BoardLogic, item.Value));

			var chestSave = GetChestsSave(board.BoardLogic, item.Value);
			if (chestSave.Count > 0)
			{
				chests.AddRange(chestSave);
				continue;
			}
			
			lifes.AddRange(GetLifeSave(board.BoardLogic, item.Value));
			
//			productions.AddRange(GetProductionSave(board.BoardLogic, item.Value));
		}
		
		pieces.Sort((a, b) => -a.Id.CompareTo(b.Id));

		completeFog = GetCompleteFog();
	}

	[OnDeserialized]
	internal void OnDeserialized(StreamingContext context)
	{
		storageSave = new Dictionary<BoardPosition, StorageSaveItem>();
		lifeSave = new Dictionary<BoardPosition, LifeSaveItem>();
		CompleteFogPositions = new List<BoardPosition>();

		if (storages != null)
		{
			foreach (var storage in storages)
			{
				storageSave.Add(storage.Position, storage);
			}
		}
		
		if (lifes != null)
		{
			foreach (var life in lifes)
			{
				lifeSave.Add(life.Position, life);
			}
		}

		if (string.IsNullOrEmpty(completeFog)) return; 
		
		var fogData = completeFog.Split(new[] {";"}, StringSplitOptions.RemoveEmptyEntries);

		foreach (var str in fogData)
		{
			CompleteFogPositions.Add(BoardPosition.Parse(str));
		}
	}

	public StorageSaveItem GetStorageSave(BoardPosition position)
	{
		StorageSaveItem item;
		
		if (storageSave == null || storageSave.TryGetValue(position, out item) == false)
		{
			return null;
		}

		storageSave.Remove(position);
		
		return item;
	}
	
	public LifeSaveItem GetLifeSave(BoardPosition position)
	{
		LifeSaveItem item;
		
		if (lifeSave == null || lifeSave.TryGetValue(position, out item) == false)
		{
			return null;
		}

		lifeSave.Remove(position);
		
		return item;
	}
	
	private PieceSaveItem GetPieceSave(int id, List<BoardPosition> positions)
	{
		var item = new PieceSaveItem {Id = id, Positions = new List<BoardPosition>()};

		foreach (var position in positions)
		{
			item.Positions.Add(position);
		}

		return item;
	}

	private List<ChestSaveItem> GetChestsSave(BoardLogicComponent logic, List<BoardPosition> positions)
	{
		var items = new List<ChestSaveItem>();

		foreach (var position in positions)
		{
			var piece = logic.GetPieceAt(position);
			
			if(piece == null) continue;

			var component = piece.GetComponent<ChestPieceComponent>(ChestPieceComponent.ComponentGuid);
			
			if(component == null) continue;

			var item = new ChestSaveItem {Id = piece.PieceType, State = component.Chest.State, Position = position, Reward = component.Chest.Reward};
			
			if (component.Chest.State == ChestState.InProgress && component.Chest.StartTime != null) item.StartTime = component.Chest.StartTime.Value;
			
			items.Add(item);
		}
		
		return items;
	}

	private List<LifeSaveItem> GetLifeSave(BoardLogicComponent logic, List<BoardPosition> positions)
	{
		var items = new List<LifeSaveItem>();
		
		foreach (var position in positions)
		{
			var piece = logic.GetPieceAt(position);
			
			if (piece == null) continue;
			
			var component = piece.GetComponent<LifeComponent>(LifeComponent.ComponentGuid);
			
			if(component == null || component.Current == 0) continue;
			
			var item = new LifeSaveItem{Step = component.Current, Position = position};
			
			items.Add(item);
		}
		
		return items;
	}

	private List<StorageSaveItem> GetStorageSave(BoardLogicComponent logic, List<BoardPosition> positions)
	{
		var items = new List<StorageSaveItem>();
		
		foreach (var position in positions)
		{
			var piece = logic.GetPieceAt(position);
			
			if (piece == null) continue;
			
			var component = piece.GetComponent<StorageComponent>(StorageComponent.ComponentGuid);
			
			if(component == null) continue;

			var item = new StorageSaveItem
			{
				Position = position,
				Filling = component.Filling,
				IsStart = component.Timer.IsExecuteable(),
				StartTime = component.Timer.StartTime
			};
			
			items.Add(item);
		}
		
		return items;
	}

	private List<ResourceSaveItem> GetResourceSave()
	{
		var items = new List<ResourceSaveItem>();
		
		if(GameDataService.Current == null) return items;

		var onBoard = GameDataService.Current.CollectionManager.ResourcesOnBoard;

		foreach (var item in onBoard)
		{
			items.Add(new ResourceSaveItem{Position = item.Key, Resources = item.Value});
		}
		
		return items;
	}

	private string GetCompleteFog()
	{
		if (GameDataService.Current == null) return null;

		var complete = GameDataService.Current.FogsManager.Completed;
		var str = new StringBuilder();

		foreach (var position in complete)
		{
			str.Append(position.ToSaveString());
			str.Append(";");
		}
		
		return str.ToString();
	}

	private List<ProductionSaveItem> GetProductionSave(BoardLogicComponent logic, List<BoardPosition> positions)
	{
		var items = new List<ProductionSaveItem>();
		
		foreach (var position in positions)
		{
			var piece = logic.GetPieceAt(position);
			
			if (piece == null) continue;
			
			var component = piece.GetComponent<ProductionComponent>(ProductionComponent.ComponentGuid);
			
			if(component == null) continue;
			
			var storage = new Dictionary<int, int>();
			
			foreach (var pair in component.Storage)
			{
				if(pair.Value.Value == 0) continue;
				
				storage.Add(pair.Key, pair.Value.Value);
			}
			
			if(storage.Count == 0) continue;
			
			var item = new ProductionSaveItem{
				Id = piece.PieceType,
				Position = position,
				State = component.State,
				StartTime = component.Timer.StartTime,
				Storage = storage
			};
			
			items.Add(item);
		}
		
		return items;
	}
}