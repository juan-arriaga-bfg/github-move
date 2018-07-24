using System.Collections.Generic;
using System;

public class ChangePieceAction : IBoardAction
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();

    public virtual int Guid
    {
        get { return ComponentGuid; }
    }

    public BoardPosition Position { get; set; }
    public int TargetPieceId { get; set; }
    
    public IBoardAction OnCompleteAction;
    public Action OnComplete;

    public bool PerformAction(BoardController gameBoardController)
    {
        gameBoardController.ActionExecutor.AddAction(new CollapsePieceToAction
        {
            Positions = new List<BoardPosition> {Position},
            To = Position,
            OnCompleteAction = new CreatePieceAtAction
            {
                At = Position,
                PieceTypeId = TargetPieceId
            }
        });
        
        if (OnComplete != null) OnComplete();
        if (OnCompleteAction != null) OnCompleteAction.PerformAction(gameBoardController);
        return true;
    }
}