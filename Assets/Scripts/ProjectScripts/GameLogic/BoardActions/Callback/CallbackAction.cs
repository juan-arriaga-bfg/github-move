using System;

public class CallbackAction : IBoardAction
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();
    public int Guid => ComponentGuid;

    public Action<BoardController> Callback;
    
    public bool PerformAction(BoardController gameBoardController)
    {
        Callback?.Invoke(gameBoardController);
        return true;
    }
}