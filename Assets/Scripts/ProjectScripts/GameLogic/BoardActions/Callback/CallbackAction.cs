using System;
using DG.Tweening;
using UnityEngine;

public class CallbackAction : IBoardAction
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();
    public int Guid => ComponentGuid;

    public Action<BoardController> Callback;
    public float Delay = -1;

    protected Sequence delaySequence = null;

    protected bool isCanceled = false;
    
    public bool PerformAction(BoardController gameBoardController)
    {
        if (isCanceled) return true;
        
        if (Delay > 0)
        {
            delaySequence = DOTween.Sequence().InsertCallback(Delay, () => Callback?.Invoke(gameBoardController));
            Delay = -1;
            return true;
        }
        
        Callback?.Invoke(gameBoardController);
        return true;
    }

    public virtual void CancelAction()
    {
        isCanceled = true;
        
        delaySequence?.Kill();
    }
}