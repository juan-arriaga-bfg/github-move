using UnityEngine;

public class BoardTimerView : UIBoardView
{
    [SerializeField] private NSText label;
    [SerializeField] private BoardProgressBarView progressBar;
    
    private TimerComponent timer;
    
    protected override void SetOfset()
    {
        CachedTransform.localPosition = controller.GetViewPositionTop(multiSize) + Ofset;
    }
    
    public override void Init(Piece piece)
    {
        base.Init(piece);
        timer = piece.GetComponent<TimerComponent>(TimerComponent.ComponentGuid);
    }
    
    private void Update()
    {
        if(timer == null) return;
        
        Change(timer.IsStarted);
        
        if(timer.IsExecuteable() == false) return;
        
        progressBar.SetProgress((float)timer.GetTime().TotalSeconds / timer.Delay);
        label.Text = timer.GetTimeLeftText(null);
    }
}