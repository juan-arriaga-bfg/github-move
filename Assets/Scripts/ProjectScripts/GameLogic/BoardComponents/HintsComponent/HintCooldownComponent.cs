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
	public static readonly int ComponentGuid = ECSManager.GetNextGuid();
	public override int Guid => ComponentGuid;

    private LockerComponent locker;
    
	public bool IsPaused => locker.IsLocked;

	private readonly TimerComponent timerArrow = new TimerComponent();
	private readonly TimerComponent timerBounce = new TimerComponent();

	private BoardController context;

	private List<int> chestsId;
	private List<int> minesId;
	private List<int> obstaclesId;
	
	private readonly List<UIBoardView> views = new List<UIBoardView>();
	
    public void Pause(object who)
    {
        Debug.Log($"[HintCooldownComponent] => Pause by {who.GetType()}");
        
        locker.Lock(who);
        UpdateLockState();
    }
    
    public void Resume(object who)
    {
        Debug.Log($"[HintCooldownComponent] => Resume by {who.GetType()}");
        
        locker.Unlock(who);
        UpdateLockState();
    }

    private void UpdateLockState()
    {
        if (timerArrow.IsPaused != locker.IsLocked)
        {
            Debug.Log($"[HintCooldownComponent] => State {(timerArrow.IsPaused ? "PAUSED" : "RESUMED")} => {(locker.IsLocked ? "PAUSED" : "RESUMED")}");
            timerArrow.IsPaused = locker.IsLocked;
        }
        else
        {
            Debug.Log($"[HintCooldownComponent] => State still {(timerArrow.IsPaused ? "PAUSED" : "RESUMED")}"); 
        }

    }

	public override void OnRegisterEntity(ECSEntity entity)
	{
	    locker = GetComponent<LockerComponent>(LockerComponent.ComponentGuid);
	    
		context = entity as BoardController;
		RegisterComponent(timerArrow, true);
		RegisterComponent(timerBounce, true);
		
		chestsId = PieceType.GetIdsByFilter(PieceTypeFilter.Chest);
		minesId = PieceType.GetIdsByFilter(PieceTypeFilter.Mine);
		obstaclesId = PieceType.GetIdsByFilter(PieceTypeFilter.Obstacle);
		
		Step(HintType.Obstacle);

		timerBounce.Delay = Random.Range(GameDataService.Current.ConstantsManager.MinDelayBounceBubble,
			GameDataService.Current.ConstantsManager.MaxDelayBounceBubble);
		
		timerBounce.OnComplete = Bounce;
	}
	
	public void Step(BoardPosition position, float offsetX = 0, float offsetY = 0)
	{
		HintArrowView.Show(position, offsetX, offsetY);
		Step(HintType.HighPriority);
	}

	public void Step(HintType type)
	{
		if(IsPaused
		   // || GameDataService.Current.QuestsManagerOld.IsThirdCompleted()
		   || type != HintType.HighPriority && type != HintType.OpenChest && timerArrow.IsStarted) return;
		
		timerArrow.Stop();
		
		timerArrow.Delay = Random.Range(GameDataService.Current.ConstantsManager.MinDelayHintArrow,
			GameDataService.Current.ConstantsManager.MaxDelayHintArrow);
		
		timerArrow.OnComplete = Hint;
		timerArrow.Start();
	}

	public void AddView(UIBoardView view)
	{
		views.Add(view);
		
		if (views.Count > 1) return;
		
		timerBounce.Start();
	}
	
	public void RemoweView(UIBoardView view)
	{
		views.Remove(view);
		
		if(views.Count > 0) return;
		
		timerBounce.Stop();
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

		positions = positions.FindAll(position =>
		{
			var piece = context.BoardLogic.GetPieceAt(position);
			return piece != null && context?.Pathfinder.CanPathToCastle(piece) == true;
		});
		
		if (positions.Count == 0) return false;
		
		positions.Shuffle();
		
		HintArrowView.Show(positions[0], 0, -0.5f);

		return true;
	}

	private void Bounce()
	{
		timerBounce.Delay = Random.Range(GameDataService.Current.ConstantsManager.MinDelayBounceBubble,
			GameDataService.Current.ConstantsManager.MaxDelayBounceBubble);
		
		timerBounce.Start();

		var view = views[Random.Range(0, views.Count)];
		view.Attention();
	}
}
