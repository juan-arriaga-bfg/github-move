using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public enum TimerViewSate
{
    Normal,
    Hide,
    Select
}

public class BoardTimerView : UIBoardView, IBoardEventListener
{
    [SerializeField] private NSText label;
    [SerializeField] private NSText price;
    
    [SerializeField] private BoardProgressBarView progressBar;
    
    [SerializeField] private Button button;
    [SerializeField] private GameObject smallButton;
    [SerializeField] private GameObject bigButton;
    [SerializeField] private GameObject hourglass;
    
    private TimerComponent timer;
    private bool isActiveHourglass;
    
    protected override ViewType Id => ViewType.BoardTimer;
    
    public override void Init(Piece piece)
    {
        base.Init(piece);
        
        Priority = defaultPriority = 10;
        
        Ofset = multiSize == 1 ? Ofset : new Vector3(0, 1.3f);
        
        SetOfset();
        SetTimer(piece.GetComponent<TimerComponent>(TimerComponent.ComponentGuid));
        SetState(TimerViewSate.Normal, 0);
        SetHourglass(false);
        
        Context.Context.BoardEvents.AddListener(this, GameEventsCodes.ClosePieceUI);
    }
    
    public void SetTimer(TimerComponent timer)
    {
        if(timer == null) return;
        if(this.timer != null) this.timer.OnExecute -= UpdateView;
        
        this.timer = timer;
        this.timer.OnExecute += UpdateView;
        this.timer.View = this;
    }
    
    public void SetState(TimerViewSate state, float duration = 0.2f)
    {
        group.DOFade(state == TimerViewSate.Hide ? 0.7f : 1, duration);
        
        button.interactable = state == TimerViewSate.Select;
        smallButton.SetActive(state == TimerViewSate.Select);
        bigButton.SetActive(state == TimerViewSate.Select);
        hourglass.SetActive(isActiveHourglass && state != TimerViewSate.Select);
    }

    public void SetHourglass(bool isActive)
    {
        if(timer == null) return;
        isActiveHourglass = isActive;
        hourglass.SetActive(isActive);
    }
    
    protected virtual void OnDestroy()
    {
        if(timer == null) return;

        timer.View = null;
        timer.OnExecute -= UpdateView;
    }

    public override void ResetViewOnDestroy()
    {
        Context.Context.BoardEvents.RemoveListener(this, GameEventsCodes.ClosePieceUI);
        timer.OnExecute -= UpdateView;
        base.ResetViewOnDestroy();
    }

    protected override void UpdateView()
    {
        if(progressBar != null) progressBar.SetProgress(timer.GetProgress());
        label.Text = timer.CompleteTime.GetTimeLeftText();
        if(smallButton.activeSelf) price.Text = timer.GetPrise().ToStringIcon(false);;
    }
    
    public void OnBoardEvent(int code, object context)
    {
        if (code != GameEventsCodes.ClosePieceUI) return;
        
        SetState(TimerViewSate.Normal, context is BoardPosition && ((BoardPosition) context).Equals(Context.CachedPosition) ? 0 : 0.2f);
    }
    
    public override void OnDrag(bool isEnd)
    {
        base.OnDrag(isEnd);
        
        if (timer == null) return;
        
        timer.IsPaused = !isEnd;
    }
    
    public void OnClick()
    {
        timer?.FastComplete();
    }
}