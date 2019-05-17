using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public enum TimerViewSate
{
    Default,
    Normal,
    Hide,
    Select,
    Free
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
    [SerializeField] private Transform timerTransform;
    
    private TimerComponent timer;
    private bool isActiveHourglass;

    private TimerViewSate timerState;
    
    protected override ViewType Id => ViewType.BoardTimer;

    protected override Vector3 offset => new Vector3(0, -0.2f);

    public override void Init(Piece piece)
    {
        base.Init(piece);
        
        Priority = defaultPriority = 10;
        
        SetTimer(Context.GetComponent<TimerComponent>(TimerComponent.ComponentGuid));
        SetHourglass(false);
        
        Context.Context.BoardEvents.AddListener(this, GameEventsCodes.ClosePieceUI);
    }
    
    public void SetTimer(TimerComponent timer)
    {
        if (timer == null) return;
        
        if (this.timer != null)
        {
            this.timer.OnTimeChanged -= UpdateView;
            this.timer.OnExecute -= FirstUpdateView;
        }
        
        this.timer = timer;
        this.timer.OnTimeChanged += UpdateView;
        this.timer.OnExecute += FirstUpdateView;
        this.timer.View = this;
        
        timerState = TimerViewSate.Default;
        SetState(TimerViewSate.Normal, 0);
    }
    
    public void SetState(TimerViewSate state, float duration = 0.2f)
    {
        if (state == timerState || timerState == TimerViewSate.Free && (state == TimerViewSate.Select || state == TimerViewSate.Hide)) return;
        
        timerState = state;
        
        var isShow = state == TimerViewSate.Select || state == TimerViewSate.Free;
        
        group.DOFade(state == TimerViewSate.Hide ? 0.7f : 1, duration);
        
        button.interactable = isShow;
        smallButton.SetActive(isShow);
        bigButton.SetActive(isShow);
        hourglass.SetActive(isActiveHourglass && state != TimerViewSate.Hide);
        UpdateView();
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
        timer.OnTimeChanged -= UpdateView;
        timer.OnExecute -= FirstUpdateView;
    }

    public override void ResetViewOnDestroy()
    {
        timerState = TimerViewSate.Default;
        Context.Context.BoardEvents.RemoveListener(this, GameEventsCodes.ClosePieceUI);
        timer.OnTimeChanged -= UpdateView;
        timer.OnExecute -= FirstUpdateView;
        base.ResetViewOnDestroy();
    }

    private void FirstUpdateView()
    {
        if (timer == null) return;
        
        UpdateView();

        if (timer.StartTime.GetTime(timer.UseUTC).TotalSeconds <= 0) return;
        
        timer.OnExecute -= FirstUpdateView;
    }

    protected override void UpdateView()
    {
        if (timer == null) return;
        if (progressBar != null) progressBar.SetProgress(timer.GetProgress());
        
        label.Text = timer.CompleteTime.GetTimeLeftText();

        if (timerState != TimerViewSate.Hide && timerState != TimerViewSate.Select) SetState(timer.IsFree() ? TimerViewSate.Free : TimerViewSate.Normal);
        if (smallButton.activeSelf) price.Text = timerState == TimerViewSate.Free ? LocalizationService.Get("common.button.free", "common.button.free") : timer.GetPrice().ToStringIcon();
    }
    
    public void OnBoardEvent(int code, object context)
    {
        if (code != GameEventsCodes.ClosePieceUI) return;
        
        SetState(TimerViewSate.Normal, context is BoardPosition position && position.Equals(Context.CachedPosition) ? 0 : 0.2f);
    }

    public override void OnSwap(bool isEnd)
    {
        base.OnSwap(isEnd);
        
        if (timer == null) return;
        
        timer.IsPaused = !isEnd;
    }

    public override void OnDrag(bool isEnd)
    {
        base.OnDrag(isEnd);
        
        if (timer == null) return;
        
        timer.IsPaused = !isEnd;
    }

    private void OffChopSound()
    {
        NSAudioService.Current.Stop(SoundId.WorkerChop);
    }
    
    public void OnClick()
    {
        if (timer.IsStarted == false) return;
        
        OffChopSound();

        var analyticsLocation = string.Empty;
        var life = Context.GetComponent<WorkplaceLifeComponent>(WorkplaceLifeComponent.ComponentGuid);

        if (life != null) analyticsLocation = life.AnalyticsLocation;
        else
        {
            var building = Context.GetComponent<PieceStateComponent>(PieceStateComponent.ComponentGuid);
            
            if (building != null) analyticsLocation = "skip_build";
        }
        
        timer.FastComplete(analyticsLocation);
    }

    public override void Attention()
    {
        DOTween.Kill(attentionUid);
        
        const int loops = 3;
        const float duration = 1.5f / (loops* 2);
        var sequence = DOTween.Sequence().SetId(attentionUid).SetEase(Ease.Linear);

        ColorUtility.TryParseHtmlString("#00D46C", out var color);
        
        sequence.SetLoops(loops);
        sequence.Insert(0f, timerTransform.DOScale(1.3f, duration));
        sequence.Insert(0f, label.TextLabel.DOColor(color, duration));
        sequence.Insert(duration, timerTransform.DOScale(1f, duration));
        sequence.Insert(duration, label.TextLabel.DOColor(Color.white, duration));
        sequence.OnKill(() => label.TextLabel.DOColor(Color.white, 0));
    }
}