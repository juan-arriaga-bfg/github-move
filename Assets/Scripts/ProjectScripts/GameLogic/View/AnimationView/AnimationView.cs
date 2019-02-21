using System;
using DG.Tweening;
using UnityEngine;

public class AnimationView: BoardElementView
{
    [SerializeField] protected int lifeTime = 5;
    
    public Action OnComplete;
    public Action<PieceBoardElementView> OnPlay;
    public Action OnLifetimeEnd;
    
    protected PieceBoardElementView context;
    
    public virtual void Play(PieceBoardElementView pieceView)
    {
        context = pieceView;
        OnPlay?.Invoke(pieceView);
        
        if (lifeTime <= 0)
            return;

        DestroyOnBoard(lifeTime);
    }

    public override void ResetViewOnDestroy()
    {
        base.ResetViewOnDestroy();
        OnLifetimeEnd?.Invoke();
        
        OnLifetimeEnd = null;
        OnComplete = null;
        OnPlay = null;
    }

    protected void CompleteAnimation()
    {
        OnComplete?.Invoke();
    }

    public override void SyncRendererLayers(BoardPosition boardPosition)
    {
        var targetPosition = new BoardPosition(boardPosition.X, boardPosition.Y, BoardLayer.FX.Layer);
        
        base.SyncRendererLayers(targetPosition);
        
    }
}