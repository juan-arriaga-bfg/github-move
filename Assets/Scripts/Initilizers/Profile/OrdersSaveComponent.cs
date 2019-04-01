﻿using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using UnityEngine;

[JsonObject(MemberSerialization.OptIn)]
public class OrdersSaveComponent : ECSEntity, IECSSerializeable
{
	public static int ComponentGuid = ECSManager.GetNextGuid();
	public override int Guid => ComponentGuid;
	
	private List<OrderSaveItem> orders;

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
		
		var logic = BoardService.Current.FirstBoard.BoardLogic;
		var positions = logic.PositionsCache.GetPiecePositionsByFilter(PieceTypeFilter.Character);
		
		orders = new List<OrderSaveItem>();
		
		if(positions.Count == 0) return;

		foreach (var position in positions)
		{
			var piece = logic.GetPieceAt(position);
			var component = piece?.GetComponent<CustomerComponent>(CustomerComponent.ComponentGuid);
			
			if(component?.Order == null || component.Order.State == OrderState.Reward) continue;
			
			var item = new OrderSaveItem
			{
				Id = component.Order.Def.Uid,//GameDataService.Current.OrdersManager.Recipes.IndexOf(component.Order.Def),
				Customer = piece.PieceType,
				State = component.Order.State,
				IsStart = component.Timer.IsExecuteable(),
				IsStartCooldown = component.Cooldown.IsExecuteable(),
				StartTime = component.Timer.StartTimeLong,
				CooldownTime = component.Cooldown.StartTimeLong,
			};
			
			orders.Add(item);
		}
	}

	[OnDeserialized]
	internal void OnDeserialized(StreamingContext context)
	{
	}

}
