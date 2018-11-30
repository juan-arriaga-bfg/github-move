using System;
using System.Collections.Generic;
using DG.Tweening;

public class ClearBoardAction : IBoardAction
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();
    public int Guid => ComponentGuid;

    public Action<BoardController> Callback;
    public float Delay = -1;
    
    public bool PerformAction(BoardController gameBoardController)
    {
        var targetPieces = gameBoardController.BoardLogic.EmptyCellsFinder.FindAllWithCondition(position =>
        {
            return gameBoardController.BoardLogic.IsEmpty(position) == false;
        });

        var collapsePieceAction = new CollapsePieceToAction {Positions = targetPieces, To = BoardPosition.Default()};
        gameBoardController.ActionExecutor.PerformAction(collapsePieceAction);
        
        return true;
    }
}
