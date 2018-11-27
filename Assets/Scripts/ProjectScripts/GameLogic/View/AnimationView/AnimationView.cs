using System;
using UnityEngine;

public class AnimationView: BoardElementView
{
    [SerializeField] protected int lifeTime = 10;
    
    private TimerComponent timer = new TimerComponent();
    
    public virtual void Play(PieceBoardElementView pieceView)
    {
        OnPlay?.Invoke(pieceView);
        if (lifeTime == 0)
            return;
        
        timer.Delay = lifeTime;
        timer.Start();
        
        timer.OnComplete += DestroySelf;
    }

    private void DestroySelf()
    {
        timer.OnComplete = null;
        OnLifetimeEnd?.Invoke();
        Context.RemoveElement(this);
    }

    public virtual void Stop()
    {
        OnStop?.Invoke();
    }

    public Action OnComplete;
    public Action<PieceBoardElementView> OnPlay;
    public Action OnStop;
    public Action OnLifetimeEnd;
}