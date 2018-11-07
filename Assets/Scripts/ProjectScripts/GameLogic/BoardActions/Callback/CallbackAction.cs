using System;
using DG.Tweening;

public class CallbackAction : IBoardAction
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();
    public int Guid => ComponentGuid;

    public Action<BoardController> Callback;
    public float Delay = -1;
    
    public bool PerformAction(BoardController gameBoardController)
    {
        if (Delay > 0)
        {
            DOTween.Sequence().InsertCallback(Delay, () => Callback?.Invoke(gameBoardController));
            Delay = -1;
            return true;
        }
        
        Callback?.Invoke(gameBoardController);
        return true;
    }
}