using System.Collections.Generic;
using UnityEngine;

public enum HintType
{
	HighPriority,
	OpenChest,
	CloseChest,
	Obstacle
}

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

	private List<int> chestsId;
	private List<int> minesId;
	private List<int> obstaclesId;
	
	public override void OnRegisterEntity(ECSEntity entity)
	{
		context = entity as BoardController;
		RegisterComponent(timer);

		chestsId = PieceType.GetIdsByFilter(PieceTypeFilter.Chest);
		minesId = PieceType.GetIdsByFilter(PieceTypeFilter.Mine);
		obstaclesId = PieceType.GetIdsByFilter(PieceTypeFilter.Obstacle);
		
		Step(HintType.Obstacle);
	}

	public void Step(BoardPosition position, float offsetX = 0, float offsetY = 0)
	{
		HintArrowView.Show(position, offsetX, offsetY);
		Step(HintType.HighPriority);
	}

	public void Step(HintType type)
	{
		if(type != HintType.HighPriority && type != HintType.OpenChest && timer.IsStarted) return;
		
		timer.Stop();
		timer.Delay = Random.Range(MinDelay, MaxDelay);
		timer.OnComplete = Hint;
		timer.Start();
	}
	
	private void Hint()
	{
		if(UIService.Get.ShowedWindows.Count > 1) return;
		
		if (Show(chestsId))
		{
			Step(HintType.OpenChest);
			return;
		}

		if (Show(minesId) || Show(obstaclesId))
		{
			
		}
	}

	private bool Show(List<int> targets)
	{
		if (targets == null) return false;
		
		var positions = new List<BoardPosition>();

		foreach (var id in targets)
		{
			positions.AddRange(context.BoardLogic.PositionsCache.GetPiecePositionsByType(id));
		}

		if (positions.Count == 0) return false;
		
		positions.Shuffle();
		
		HintArrowView.Show(positions[0], 0, -0.5f);

		return true;
	}
}
