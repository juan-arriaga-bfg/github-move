using System.Collections.Generic;
using UnityEngine;

public class CharactersLogicComponent : ECSEntity
{
	public static readonly int ComponentGuid = ECSManager.GetNextGuid();
	public override int Guid => ComponentGuid;
	
	private BoardLogicComponent context;
	
	private readonly TimerComponent timer = new TimerComponent();
	
	private List<int> ids;
	
	public override void OnRegisterEntity(ECSEntity entity)
	{
		context = entity as BoardLogicComponent;
		ids = PieceType.GetIdsByFilter(PieceTypeFilter.Character);
		RegisterComponent(timer);
		Waiting();
	}
	
	private void Waiting()
	{
		timer.Delay = GameDataService.Current.ConstantsManager.CreateManaDelay;
		timer.OnComplete = CreateMana;
		timer.Start();
	}
	
	public void Step()
	{
		return;
		foreach (var id in ids)
		{
			var positions = context.PositionsCache.GetPiecePositionsByType(id);

			foreach (var position in positions)
			{
			    // Do not move locked chars
			    if (context.IsLockedCell(position))
			    {
			        continue;
			    }

				context.Context.ActionExecutor.AddAction(new MoveCharacterAction{From = position});
			}
		}
	}

	private void CreateMana()
	{
		var positions = new List<BoardPosition>();

		foreach (var id in ids)
		{
			positions.AddRange(context.PositionsCache.GetPiecePositionsByType(id));
		}
		
		positions.Shuffle();

		foreach (var position in positions)
		{
			var piece = context.GetPieceAt(position);
			var storage = piece?.GetComponent<StorageComponent>(StorageComponent.ComponentGuid);
			
			if(storage == null || storage.IsFilled || storage.Timer.IsStarted) continue;
			
			storage.Timer.Start();
			break;
		}
		
		Waiting();
	}
}