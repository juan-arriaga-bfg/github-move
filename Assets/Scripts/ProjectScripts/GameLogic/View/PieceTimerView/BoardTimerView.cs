﻿using System.Linq;
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
    
    private TimerComponent timer;
    private bool isActiveHourglass;

    private TimerViewSate timerState;
    
    protected override ViewType Id => ViewType.BoardTimer;
    
    public override void Init(Piece piece)
    {
        base.Init(piece);
        
        Priority = defaultPriority = 10;
        
        Ofset = multiSize == 1 ? Ofset : new Vector3(0, 1.3f);
        
        SetOfset();
        SetTimer(Context.GetComponent<TimerComponent>(TimerComponent.ComponentGuid));
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
        
        timerState = TimerViewSate.Default;
        SetState(TimerViewSate.Normal, 0);
    }
    
    public void SetState(TimerViewSate state, float duration = 0.2f)
    {
        if(state == timerState || timerState == TimerViewSate.Free) return;
        
        timerState = state;
        
        var isShow = state == TimerViewSate.Select || state == TimerViewSate.Free;
        
        group.DOFade(state == TimerViewSate.Hide ? 0.7f : 1, duration);
        
        button.interactable = isShow;
        smallButton.SetActive(isShow);
        bigButton.SetActive(isShow);
        hourglass.SetActive(isActiveHourglass && state != TimerViewSate.Hide);
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
        timerState = TimerViewSate.Default;
        Context.Context.BoardEvents.RemoveListener(this, GameEventsCodes.ClosePieceUI);
        timer.OnExecute -= UpdateView;
        base.ResetViewOnDestroy();
    }

    protected override void UpdateView()
    {
        if (timer == null) return;
        
        if(progressBar != null) progressBar.SetProgress(timer.GetProgress());
        label.Text = timer.CompleteTime.GetTimeLeftText();
        
        if (timer.CompleteTime.GetTimeLeft().TotalSeconds <= GameDataService.Current.ConstantsManager.FreeTimeLimit)
        {
            SetState(TimerViewSate.Free);
        }
        
        if(smallButton.activeSelf) price.Text = timerState == TimerViewSate.Free ? LocalizationService.Get("common.button.free", "common.button.free") : timer.GetPrise().ToStringIcon();
    }
    
    public void OnBoardEvent(int code, object context)
    {
        if (code != GameEventsCodes.ClosePieceUI) return;
        
        SetState(TimerViewSate.Normal, context is BoardPosition && ((BoardPosition) context).Equals(Context.CachedPosition) ? 0 : 0.2f);
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
        NSAudioService.Current.Stop(SoundId.worker_chop);
    }
    
    public void OnClick()
    {
        OffChopSound();
        timer?.FastComplete();
    }
}