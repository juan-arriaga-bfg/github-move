using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Newtonsoft.Json;

[JsonObject(MemberSerialization.OptIn)]
public class FieldDefComponent : ECSEntity, IECSSerializeable
{
	public static int ComponentGuid = ECSManager.GetNextGuid();
	public override int Guid => ComponentGuid;

	private List<PieceSaveItem> pieces;
	private List<RewardsSaveItem> rewards;
	private List<LifeSaveItem> lifes;
	private List<BuildingSaveItem> buildings;
	private AreaAccessSaveItem areaAccess;
	
	private string completeFogs;
	private string movedMines;
	private string remowedMines;
	
	[JsonProperty] public string ResetMarketStartTime;
	
	[JsonProperty]
	public List<PieceSaveItem> Pieces
	{
		get { return pieces; }
		set { pieces = value; }
	}
	
	[JsonProperty]
	public List<RewardsSaveItem> Rewards
	{
		get { return rewards; }
		set { rewards = value; }
	}
	
	[JsonProperty]
	public List<LifeSaveItem> Lifes
	{
		get { return lifes; }
		set { lifes = value; }
	}
	
	[JsonProperty]
	public string CompleteFogs
	{
		get { return completeFogs; }
		set { completeFogs = value; }
	}
	
	[JsonProperty]
	public string MovedMines
	{
		get { return movedMines; }
		set { movedMines = value; }
	}
	
	[JsonProperty]
	public string RemowedMines
	{
		get { return remowedMines; }
		set { remowedMines = value; }
	}
	
	[JsonProperty]
	public List<BuildingSaveItem> Buildings
	{
		get { return buildings; }
		set { buildings = value; }
	}

	[JsonProperty]
	public AreaAccessSaveItem AreaAccess
	{
		get { return areaAccess; }
		set { areaAccess = value; }
	}

	public List<BoardPosition> CompleteFogPositions;
	public List<BoardPosition> MovedMinePositions;
	public List<int> RemovedMinePositions;
	
	private Dictionary<BoardPosition, RewardsSaveItem> rewardsSave;
	private Dictionary<BoardPosition, LifeSaveItem> lifeSave;
	private Dictionary<BoardPosition, BuildingSaveItem> buildingSave;
	
	[OnSerializing]
	internal void OnSerialization(StreamingContext context)
	{
		if(BoardService.Current == null) return;
		
		var board = BoardService.Current.FirstBoard;
		var cash = board.BoardLogic.PositionsCache.Cache;
		
		ResetMarketStartTime = board.MarketLogic.Timer.StartTimeLong.ToString();
		
		if(cash.Count == 0) return;
		
		pieces = new List<PieceSaveItem>();
		rewards = new List<RewardsSaveItem>();
		lifes = new List<LifeSaveItem>();
		buildings = new List<BuildingSaveItem>();
		
		foreach (var item in cash)
		{
			if(item.Value.Count == 0) continue;
			
			pieces.Add(GetPieceSave(item.Key, item.Value));
			rewards.AddRange(GetRewardsSave(board.BoardLogic, item.Value));
			lifes.AddRange(GetLifeSave(board.BoardLogic, item.Value));
			buildings.AddRange(GetBuildingSave(board.BoardLogic, item.Value));
		}
		
		pieces.Sort((a, b) => -a.Id.CompareTo(b.Id));

		completeFogs = PositionsToString(GameDataService.Current.FogsManager.ClearedFogPositions.Keys.ToList());
		movedMines = PositionsToString(GameDataService.Current.MinesManager.Moved);

		if (GameDataService.Current == null) return;
		
		var str = new StringBuilder();

		foreach (var id in GameDataService.Current.MinesManager.Removed)
		{
			str.Append(id);
			str.Append(",");
		}
		
		remowedMines = str.ToString();

		AreaAccess = board.AreaAccessController.GetSaveItem();
	}

	[OnDeserialized]
	internal void OnDeserialized(StreamingContext context)
	{
		rewardsSave = new Dictionary<BoardPosition, RewardsSaveItem>();
		lifeSave = new Dictionary<BoardPosition, LifeSaveItem>();
		buildingSave = new Dictionary<BoardPosition, BuildingSaveItem>();
		
		CompleteFogPositions = StringToPositions(completeFogs);
		MovedMinePositions = StringToPositions(movedMines);
		RemovedMinePositions = new List<int>();
		
		if (rewards != null)
		{
			foreach (var reward in rewards)
			{
				rewardsSave.Add(reward.Position, reward);
			}
		}
		
		if (lifes != null)
		{
			foreach (var life in lifes)
			{
				lifeSave.Add(life.Position, life);
			}
		}
		
		if (buildings != null)
		{
			foreach (var building in buildings)
			{
				buildingSave.Add(building.Position, building);
			}
		}
		
		if (string.IsNullOrEmpty(remowedMines) == false)
		{
			var data = remowedMines.Split(new[] {","}, StringSplitOptions.RemoveEmptyEntries);

			foreach (var str in data)
			{
				RemovedMinePositions.Add(int.Parse(str));
			}
		}
		
	}
	
	public RewardsSaveItem GetRewardsSave(BoardPosition position)
	{
		RewardsSaveItem item;
		
		if (rewardsSave == null || rewardsSave.TryGetValue(position, out item) == false)
		{
			return null;
		}

		rewardsSave.Remove(position);
		
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
	
	public BuildingSaveItem GetBuildingSave(BoardPosition position)
	{
		BuildingSaveItem item;
		
		if (buildingSave == null || buildingSave.TryGetValue(position, out item) == false)
		{
			return null;
		}

		buildingSave.Remove(position);
		
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

	private List<RewardsSaveItem> GetRewardsSave(BoardLogicComponent logic, List<BoardPosition> positions)
	{
		var items = new List<RewardsSaveItem>();

		foreach (var position in positions)
		{
			var piece = logic.GetPieceAt(position);
			var component = piece?.GetComponent<RewardsStoreComponent>(RewardsStoreComponent.ComponentGuid);
			
			if(component == null) break;
			
			var item = component.Save();
			
			if(item == null) continue;
			
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
			var component = piece?.GetComponent<WorkplaceLifeComponent>(WorkplaceLifeComponent.ComponentGuid);
			
			if(component == null) break;

			var item = component.Save();
			
			if(item == null) continue;
			
			items.Add(item);
		}
		
		return items;
	}

	private string PositionsToString(List<BoardPosition> positions)
	{
		if (GameDataService.Current == null) return null;
		
		var str = new StringBuilder();

		foreach (var position in positions)
		{
			str.Append(position.ToSaveString());
			str.Append(";");
		}
		
		return str.ToString();
	}

	private List<BoardPosition> StringToPositions(string value)
	{
		var positions = new List<BoardPosition>();
		
		if (string.IsNullOrEmpty(value)) return positions;
		
		var data = value.Split(new[] {";"}, StringSplitOptions.RemoveEmptyEntries);

		foreach (var str in data)
		{
			positions.Add(BoardPosition.Parse(str));
		}
		
		return positions;
	}
	
	private List<BuildingSaveItem> GetBuildingSave(BoardLogicComponent logic, List<BoardPosition> positions)
	{
		var items = new List<BuildingSaveItem>();
		
		foreach (var position in positions)
		{
			var piece = logic.GetPieceAt(position);
			var component = piece?.GetComponent<PieceStateComponent>(PieceStateComponent.ComponentGuid);
			
			if(component == null || component.State == BuildingState.Complete) continue;
			
			var item = new BuildingSaveItem{
				Id = piece.PieceType,
				Position = position,
				State = component.State,
				StartTime = component.Timer.StartTimeLong,
			};
			
			items.Add(item);
		}
		
		return items;
	}
}