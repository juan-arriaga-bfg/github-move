using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;

[JsonObject(MemberSerialization.OptIn)]
public class OrdersSaveComponent : ECSEntity, IECSSerializeable
{
	public static int ComponentGuid = ECSManager.GetNextGuid();
	public override int Guid => ComponentGuid;
	
	private List<OrderSaveItem> orders;

	public override void OnRegisterEntity(ECSEntity entity)
	{
	}

	[JsonProperty]
	public List<OrderSaveItem> Orders
	{
		get { return orders; }
		set { orders = value; }
	}

	[OnSerializing]
	internal void OnSerialization(StreamingContext context)
	{
		if(BoardService.Current == null) return;
		
		var logic = BoardService.Current.GetBoardById(0).BoardLogic;
		var positions = logic.PositionsCache.GetPiecePositionsByFilter(PieceTypeFilter.Character);
		
		orders = new List<OrderSaveItem>();
		
		if(positions.Count == 0) return;

		foreach (var position in positions)
		{
			var piece = logic.GetPieceAt(position);
			var component = piece?.GetComponent<CustomerComponent>(CustomerComponent.ComponentGuid);
			
			if(component?.Order == null) continue;
			
			var item = new OrderSaveItem
			{
				Id = GameDataService.Current.OrdersManager.Recipes.IndexOf(component.Order.Def),
				Customer = piece.PieceType,
				State = component.Order.State,
				IsStart = component.Timer.IsExecuteable(),
				IsStartCooldown = component.Cooldown.IsExecuteable(),
				StartTime = component.Timer.StartTimeLong,
				CooldownTime = component.Cooldown.StartTimeLong
			};
			
			orders.Add(item);
		}
	}

	[OnDeserialized]
	internal void OnDeserialized(StreamingContext context)
	{
		
	}
}
