using System;
using UnityEngine;

public class ChangeObstacleStateView : IWBaseMonoBehaviour
{
    [SerializeField] private GameObject lockGo;
    [SerializeField] private GameObject unlockGo;
    [SerializeField] private GameObject inProgresGo;
    [SerializeField] private GameObject openGo;
    
    [SerializeField] private NSText message;
    [SerializeField] private NSText price;

    private PieceBoardElementView context;
    private Obstacle obstacle;

    private ObstacleConditionDelay condition;

    private ObstacleState cachedState;
    
    public void Init(PieceBoardElementView context)
    {
        this.context = context;
        lockGo.SetActive(false);
        unlockGo.SetActive(false);
        inProgresGo.SetActive(false);
        openGo.SetActive(false);
    }

    private void Update()
    {
        if (context == null || context.Piece == null) return;

        if (obstacle == null)
        {
            obstacle = context.Piece.Context.ObstaclesLogic.GetObstacle(context.Piece.CachedPosition);
            if (obstacle == null) return;
        }

        if (cachedState == ObstacleState.InProgres)
        {
            UpdateTimer();
        }
        
        if(cachedState == obstacle.State) return;

        cachedState = obstacle.State;
        
        lockGo.SetActive(cachedState == ObstacleState.Lock);
        unlockGo.SetActive(cachedState == ObstacleState.Unlock);
        inProgresGo.SetActive(cachedState == ObstacleState.InProgres);
        openGo.SetActive(cachedState == ObstacleState.Open);
        
        message.Text = cachedState == ObstacleState.Unlock ? "Help" : (cachedState == ObstacleState.Open ? "Complete" : "");
        price.Text = obstacle.Def.Price.Amount.ToString();
    }

    private void UpdateTimer()
    {
        if (condition == null)
        {
            condition = obstacle.GetOpenConditions<ObstacleConditionDelay>()[0];
        }
		
        if (condition == null) return;

        var currentSeconds = (float)(DateTime.Now - condition.StartTime).TotalSeconds;
        var targetSeconds = condition.Delay - condition.Bonus;
        
        if (currentSeconds > targetSeconds)
        {
            message.Text = "00:00";
            return;
        }
        
        message.Text = GetFormattedTime(TimeSpan.FromSeconds(targetSeconds - currentSeconds));
    }
    
    public string GetFormattedTime(TimeSpan time)
    {
        var str = "";

        if ((int) time.TotalHours > 0)
        {
            str = string.Format("{0}:{1}", time.Hours, (time.Minutes > 9 ? "" : "0") + time.Minutes);
        }
        else
        {
            str = string.Format("{0}:{1}", (time.Minutes > 9 ? "" : "0") + time.Minutes,
                (time.Seconds > 9 ? "" : "0") + time.Seconds);
        }

        return str;
    }
}