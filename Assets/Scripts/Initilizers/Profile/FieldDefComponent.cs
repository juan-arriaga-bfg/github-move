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
	private List<ObstacleSaveItem> obstacles;
	private List<StorageSaveItem> storages;
	private List<ResourceSaveItem> resources;
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
	public List<ObstacleSaveItem> Obstacles
	{
		get { return obstacles; }
		set { obstacles = value; }
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

	public List<BoardPosition> CompleteFogPositions;
	
	public Dictionary<BoardPosition, StorageSaveItem> StorageSave;
	
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
		obstacles = new List<ObstacleSaveItem>();
		storages = new List<StorageSaveItem>();
		
		resources = GetResourceSave();
		
		foreach (var item in cash)
		{
			if(item.Value.Count == 0) continue;
			
			pieces.Add(GetPieceSave(item.Key, item.Value));

			var id = match.GetFirst(item.Key);
			
			storages.AddRange(GetStorageSave(board.BoardLogic, item.Value));

			if (id == PieceType.Chest1.Id)
			{
				chests.AddRange(GetChestsSave(board.BoardLogic, item.Value));
				continue;
			}

			if (id == PieceType.O1.Id || id == PieceType.OX1.Id)
			{
				obstacles.AddRange(GetObstacleSave(board.BoardLogic, item.Value));
				continue;
			}
		}
		
		pieces.Sort((a, b) => -a.Id.CompareTo(b.Id));

		completeFog = GetCompleteFog();
	}

	[OnDeserialized]
	internal void OnDeserialized(StreamingContext context)
	{
		StorageSave = new Dictionary<BoardPosition, StorageSaveItem>();
		CompleteFogPositions = new List<BoardPosition>();

		foreach (var storage in storages)
		{
			StorageSave.Add(storage.Position, storage);
		}
		
		var fogData = completeFog.Split(new string[] {";"}, StringSplitOptions.RemoveEmptyEntries);

		foreach (var str in fogData)
		{
			CompleteFogPositions.Add(BoardPosition.Parse(str));
		}
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
			
			if(component == null || component.Chest.State != ChestState.InProgress && component.Chest.State != ChestState.Open) continue;

			var item = new ChestSaveItem {Id = piece.PieceType, State = component.Chest.State, Position = position};
			
			if (component.Chest.State == ChestState.InProgress && component.Chest.StartTime != null) item.StartTime = component.Chest.StartTime.Value;
			
			items.Add(item);
		}
		
		return items;
	}

	private List<ObstacleSaveItem> GetObstacleSave(BoardLogicComponent logic, List<BoardPosition> positions)
	{
		var items = new List<ObstacleSaveItem>();
		
		foreach (var position in positions)
		{
			var piece = logic.GetPieceAt(position);
			
			if (piece == null) continue;
			
			var component = piece.GetComponent<ObstacleLifeComponent>(ObstacleLifeComponent.ComponentGuid);
			
			if(component == null || component.Current == 0) continue;
			
			var item = new ObstacleSaveItem{Step = component.Current, Position = position};
			
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
			
			var item = new StorageSaveItem{Position = position, Filling = component.Filling, StartTime = component.Timer.StartTime};
			
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
}