using UnityEngine;

public class BoardTimerView : UIBoardView
{
    [SerializeField] private NSText label;
    [SerializeField] private BoardProgressBarView progressBar;
    
    private TimerComponent timer;
    
    protected override void SetOfset()
    {
        if (multiSize == 1)
        {
            base.SetOfset();
            return;
        }
        
        CachedTransform.localPosition = controller.GetViewPositionTop(multiSize) + Ofset;
    }
    
    public override void Init(Piece piece)
    {
        base.Init(piece);
        timer = piece.GetComponent<TimerComponent>(TimerComponent.ComponentGuid);

        timer.OnExecute += UpdateView;
    }

    public override void ResetViewOnDestroy()
    {
        timer.OnExecute -= UpdateView;
        base.ResetViewOnDestroy();
    }

    protected override void UpdateView()
    {
        if(progressBar != null) progressBar.SetProgress((float)timer.GetTime().TotalSeconds / timer.Delay);
        label.Text = timer.GetTimeLeftText(null);
    }
}