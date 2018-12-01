using System.Collections.Generic;

public class LockCellAction : IBoardAction
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();
    public virtual int Guid => ComponentGuid;

    public FieldControllerComponent Locker;
    public List<BoardPosition> Positions { get; set; }

    public bool PerformAction(BoardController gameBoardController)
    {
        foreach (var position in Positions)
        {
            var target = position;

            target.Z = BoardLayer.Piece.Layer;
            
            if(gameBoardController.BoardLogic.IsEmpty(target) == false) continue;
            
            gameBoardController.BoardLogic.LockCell(target, Locker);
        }
        
        return true;
    }
}