using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using UnityEngine;

[JsonObject(MemberSerialization.OptIn)]
public class FieldDefComponent : ECSEntity, IECSSerializeable 
{
	public static int ComponentGuid = ECSManager.GetNextGuid();

	public override int Guid { get { return ComponentGuid; } }

	private List<PieceSaveItem> pieces;
	private List<ChestSaveItem> chests;

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
		
		foreach (var item in cash)
		{
			if(item.Value.Count == 0) continue;
			
			pieces.Add(GetPieceSave(item.Key, item.Value));

			if (match.GetFirst(item.Key) == PieceType.Chest1.Id)
			{
				chests.AddRange(GetChestsSave(board.BoardLogic, item.Value));
			}
		}
		
		pieces.Sort((a, b) => a.Id.CompareTo(b.Id));
	}
	
	[OnDeserialized]
	internal void OnDeserialized(StreamingContext context)
	{
		if(GameDataService.Current == null) return;
		
		var chestsManager = GameDataService.Current.ChestsManager;

		foreach (var item in Chests)
		{
			chestsManager.AddToBoard(item.Position, item.Id, true);
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
}
