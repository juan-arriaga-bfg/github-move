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
    private Action OnComplete;
    
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
            
            OnComplete = () => HintArrowView.AddHint(context.Piece.CachedPosition);
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

        if (cachedState == ObstacleState.Open && OnComplete != null)
        {
            OnComplete();
            OnComplete = null;
        }
    }

    private void UpdateTimer()
    {
        if (condition == null)
        {
            condition = obstacle.GetOpenConditions<ObstacleConditionDelay>()[0];
        }
		
        if (condition == null) return;
        
        message.Text = TimeFormat(condition.CompleteTime - DateTime.Now);
    }
    
    private string TimeFormat(TimeSpan time)
    {
        if ((int) time.TotalSeconds == 0) return "00:00";
        
        return (int) time.TotalHours > 0
            ? string.Format("{0:00}:{1:00}:{2:00}", time.Hours, time.Minutes, time.Seconds) 
            : string.Format("{0:00}:{1:00}", time.Minutes, time.Seconds);
    }
}