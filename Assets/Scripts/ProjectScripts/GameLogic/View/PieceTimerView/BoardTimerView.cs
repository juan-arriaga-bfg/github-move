using UnityEngine;

public class BoardTimerView : IWBaseMonoBehaviour
{
    [SerializeField] private GameObject timerGo;
    [SerializeField] private NSText label;
    [SerializeField] private BoardProgressBarView progressBar;
    
    private PieceBoardElementView context;
    private TimerComponent timer;
    
    public void Init(PieceBoardElementView context)
    {
        this.context = context;
        timerGo.SetActive(false);
    }
    
    private void OnDisable()
    {
        timer = null;
    }
    
    private void Update()
    {
        if (context == null || context.Piece == null) return;

        if (timer == null)
        {
            timer = context.Piece.GetComponent<TimerComponent>(TimerComponent.ComponentGuid);
            
            if (timer == null) return;
        }
        
        timerGo.SetActive(timer.IsStarted);
        
        if(timer.IsExecuteable() == false) return;
        
        progressBar.SetProgress((float)timer.GetTime().TotalSeconds / timer.Delay);
        label.Text = timer.GetTimeLeftText(null);
    }
}