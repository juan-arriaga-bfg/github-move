using UnityEngine;

public class BoardTimerView : UIBoardView
{
    [SerializeField] private NSText label;
    [SerializeField] private BoardProgressBarView progressBar;
    
    private TimerComponent timer;
    
    protected override ViewType Id => ViewType.BoardTimer;

    public override void Init(Piece piece)
    {
        base.Init(piece);
        
        Ofset = multiSize == 1 ? Ofset : new Vector3(0, 1.3f);
        SetOfset();

        Priority = defaultPriority = 10;
        SetTimer(piece.GetComponent<TimerComponent>(TimerComponent.ComponentGuid));
    }
    
    public void SetTimer(TimerComponent timer)
    {
        if(timer == null) return;
        if(this.timer != null) this.timer.OnExecute -= UpdateView;
        
        this.timer = timer;
        this.timer.OnExecute += UpdateView;
    }

    protected virtual void OnDestroy()
    {
        if(timer != null) timer.OnExecute -= UpdateView;
    }

    public override void ResetViewOnDestroy()
    {
        timer.OnExecute -= UpdateView;
        base.ResetViewOnDestroy();
    }

    protected override void UpdateView()
    {
        if(progressBar != null) progressBar.SetProgress(timer.GetProgress());
        label.Text = timer.CompleteTime.GetTimeLeftText();
    }
}