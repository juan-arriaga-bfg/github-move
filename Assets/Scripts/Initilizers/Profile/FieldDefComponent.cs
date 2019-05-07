using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;

public interface IFieldDefComponent
{
    FieldDefComponent FieldDef { get; }
}

public partial class UserProfile : IFieldDefComponent
{
    protected FieldDefComponent fieldDefComponent;

    [JsonIgnore]
    public FieldDefComponent FieldDef
    {
        get
        {
            if (fieldDefComponent == null)
            {
                fieldDefComponent = GetComponent<FieldDefComponent>(FieldDefComponent.ComponentGuid);
            }

            return fieldDefComponent;
        }
    }
}

[JsonObject(MemberSerialization.OptIn)]
public class FieldDefComponent : BaseSaveComponent, IECSSerializeable, IProfileSaveComponent
{
	public bool AllowDataCollect { get; set; }

	public static int ComponentGuid = ECSManager.GetNextGuid();
	public override int Guid => ComponentGuid;

	private int vipIslandState;
	
	private List<PieceSaveItem> pieces;
	private List<RewardsSaveItem> rewards;
	private List<LifeSaveItem> lives;
	private List<LoopSaveItem> loops;
	private List<BuildingSaveItem> buildings;
	
	[JsonProperty]
	public int VIPIslandState
	{
		get { return vipIslandState; }
		set { vipIslandState = value; }
	}
	
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
	public List<LoopSaveItem> Loops
	{
		get { return loops; }
		set { loops = value; }
	}
	
	[JsonProperty]
	public List<BuildingSaveItem> Buildings
	{
		get { return buildings; }
		set { buildings = value; }
	}
	
	private Dictionary<BoardPosition, RewardsSaveItem> rewardsSave;
	private Dictionary<BoardPosition, LifeSaveItem> lifeSave;
	private Dictionary<BoardPosition, BuildingSaveItem> buildingSave;
	
	[OnSerializing]
	internal void OnSerialization(StreamingContext context)
	{
		if (!AllowDataCollect)
		{
			return;
		}
		
		if(BoardService.Current == null) return;
		
		var board = BoardService.Current.FirstBoard;
		var cache = board.BoardLogic.PositionsCache.Cache;

		vipIslandState = (int) board.BoardLogic.VIPIslandLogic.State;
		
		if(cache.Count == 0) return;
		
		pieces = new List<PieceSaveItem>();
		rewards = new List<RewardsSaveItem>();
		lives = new List<LifeSaveItem>();
		buildings = new List<BuildingSaveItem>();
		
		foreach (var item in cache)
		{
			if(item.Value.Count == 0) continue;

			// Exclude fog from save. Fog always will be reloaded from the configs.
            if (item.Key != PieceType.Fog.Id) pieces.Add(GetPieceSave(item.Key, item.Value));
            
			rewards.AddRange(GetRewardsSave(board.BoardLogic, item.Value));
			lives.AddRange(GetLifeSave(board.BoardLogic, item.Value));
			buildings.AddRange(GetBuildingSave(board.BoardLogic, item.Value));
		}
		
		pieces.Sort((a, b) => -a.Id.CompareTo(b.Id));
	}

	[OnDeserialized]
	internal void OnDeserialized(StreamingContext context)
	{
		ReplaceRewardsSave();
		ReplaceLifeSave();
		ReplaceBuildingSave();
	}

	public void ReplaceRewardsSave()
	{
		rewardsSave = new Dictionary<BoardPosition, RewardsSaveItem>();

		if (rewards == null) return;
		
		foreach (var reward in rewards)
		{
			rewardsSave.Add(reward.Position, reward);
		}
	}
	
	public RewardsSaveItem GetRewardsSave(BoardPosition position)
	{
		if (rewardsSave == null || rewardsSave.TryGetValue(position, out var item) == false)
		{
			return null;
		}

		rewardsSave.Remove(position);
		
		return item;
	}

	public void ReplaceLifeSave()
	{
		lifeSave = new Dictionary<BoardPosition, LifeSaveItem>();
		
		if (lives == null) return;
		
		foreach (var life in lives)
		{
			lifeSave.Add(life.Position, life);
		}
	}
	
	public LifeSaveItem GetLifeSave(BoardPosition position)
	{
		if (lifeSave == null || lifeSave.TryGetValue(position, out var item) == false)
		{
			return null;
		}

		lifeSave.Remove(position);
		
		return item;
	}

	public void ReplaceBuildingSave()
	{
		buildingSave = new Dictionary<BoardPosition, BuildingSaveItem>();
		
		if (buildings == null) return;
		
		foreach (var building in buildings)
		{
			buildingSave.Add(building.Position, building);
		}
	}
	
	public BuildingSaveItem GetBuildingSave(BoardPosition position)
	{
		if (buildingSave == null || buildingSave.TryGetValue(position, out var item) == false)
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