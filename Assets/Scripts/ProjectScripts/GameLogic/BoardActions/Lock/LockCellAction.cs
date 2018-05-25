using System.Collections.Generic;

public class LockCellAction : IBoardAction
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();
	
    public virtual int Guid
    {
        get { return ComponentGuid; }
    }

    public FieldControllerComponent Locker;
    public List<BoardPosition> Positions { get; set; }

    public bool PerformAction(BoardController gameBoardController)
    {
        foreach (var position in Positions)
        {
            if(gameBoardController.BoardLogic.IsEmpty(position) == false) continue;
            
            gameBoardController.BoardLogic.LockCell(position, Locker);
        }
        
        return true;
    }
}