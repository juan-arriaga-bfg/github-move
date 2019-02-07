using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using Newtonsoft.Json;

[JsonObject(MemberSerialization.OptIn)]
public class FieldDefComponent : BaseSaveComponent, IECSSerializeable
{
	public static int ComponentGuid = ECSManager.GetNextGuid();
	public override int Guid => ComponentGuid;

	private List<PieceSaveItem> pieces;
	private List<RewardsSaveItem> rewards;
	private List<LifeSaveItem> lives;
	private List<BuildingSaveItem> buildings;
	private AreaAccessSaveItem areaAccess;
	
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
	public List<LifeSaveItem> Lives
	{
		get { return lives; }
		set { lives = value; }
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
	
	private Dictionary<BoardPosition, RewardsSaveItem> rewardsSave;
	private Dictionary<BoardPosition, LifeSaveItem> lifeSave;
	private Dictionary<BoardPosition, BuildingSaveItem> buildingSave;
	
	[OnSerializing]
	internal void OnSerialization(StreamingContext context)
	{
		if(BoardService.Current == null) return;
		
		var board = BoardService.Current.FirstBoard;
		var cash = board.BoardLogic.PositionsCache.Cache;
		
		if(cash.Count == 0) return;
		
		pieces = new List<PieceSaveItem>();
		rewards = new List<RewardsSaveItem>();
		lives = new List<LifeSaveItem>();
		buildings = new List<BuildingSaveItem>();
		
		foreach (var item in cash)
		{
			if(item.Value.Count == 0) continue;
			
			pieces.Add(GetPieceSave(item.Key, item.Value));
			rewards.AddRange(GetRewardsSave(board.BoardLogic, item.Value));
			lives.AddRange(GetLifeSave(board.BoardLogic, item.Value));
			buildings.AddRange(GetBuildingSave(board.BoardLogic, item.Value));
		}
		
		pieces.Sort((a, b) => -a.Id.CompareTo(b.Id));
		AreaAccess = board.AreaAccessController.GetSaveItem();
	}

	[OnDeserialized]
	internal void OnDeserialized(StreamingContext context)
	{
		rewardsSave = new Dictionary<BoardPosition, RewardsSaveItem>();
		lifeSave = new Dictionary<BoardPosition, LifeSaveItem>();
		buildingSave = new Dictionary<BoardPosition, BuildingSaveItem>();
		
		if (rewards != null)
		{
			foreach (var reward in rewards)
			{
				rewardsSave.Add(reward.Position, reward);
			}
		}
		
		if (lives != null)
		{
			foreach (var life in lives)
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