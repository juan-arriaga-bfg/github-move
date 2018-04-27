using System.Collections.Generic;
using UnityEngine;

public class HintCooldownComponent : ECSEntity
{
	private const int MinDelay = 30;
	private const int MaxDelay = 60;
	
	public static readonly int ComponentGuid = ECSManager.GetNextGuid();
	
	public override int Guid
	{
		get { return ComponentGuid; }
	}
    
	private TimerComponent timer = new TimerComponent();

	private BoardController context;
	
	public override void OnRegisterEntity(ECSEntity entity)
	{
		context = entity as BoardController;
		RegisterComponent(timer);
		Step();
	}
	
	private void Step()
	{
		timer.Delay = Random.Range(MinDelay, MaxDelay);
		timer.OnComplete = Hint;
		timer.Start();
	}
	
	private void Hint()
	{
		Step();
		
		if(UIService.Get.ShowedWindows.Count > 2) return;
		
		var open = new List<BoardPosition>();
		var close = new List<BoardPosition>();
		
		for (var i = PieceType.Chest1.Id; i <= PieceType.Chest9.Id; i++)
		{
			var positions = context.BoardLogic.PositionsCache.GetPiecePositionsByType(i);

			foreach (var position in positions)
			{
				var piece = context.BoardLogic.GetPieceAt(position);
				var chestComponent = piece.GetComponent<ChestPieceComponent>(ChestPieceComponent.ComponentGuid);
				
				if(chestComponent == null) return;

				if (chestComponent.Chest.State == ChestState.Open)
				{
					open.Add(position);
					continue;
				}

				if (chestComponent.Chest.State == ChestState.Close)
				{
					close.Add(position);
				}
			}
		}
		
		if (open.Count > 0)
		{
			open.Shuffle();
			HintArrowView.Show(open[0], 0, -0.5f);
			return;
		}

		if (close.Count > 0)
		{
			close.Shuffle();
			HintArrowView.Show(close[0], 0, -0.5f);
			return;
		}
		
		var obstacle = new List<BoardPosition>();

		for (var i = PieceType.O1.Id; i <= PieceType.O5.Id; i++)
		{
			obstacle.AddRange(context.BoardLogic.PositionsCache.GetPiecePositionsByType(i));
		}
		
		for (var i = PieceType.OX1.Id; i <= PieceType.OX5.Id; i++)
		{
			obstacle.AddRange(context.BoardLogic.PositionsCache.GetPiecePositionsByType(i));
		}

		if (obstacle.Count == 0) return;
		
		obstacle.Shuffle();
		HintArrowView.Show(obstacle[0], 0, -0.5f);
	}
}
