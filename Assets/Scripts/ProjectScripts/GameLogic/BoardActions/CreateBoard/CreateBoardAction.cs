public class CreateBoardAction : IBoardAction
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();
    public virtual int Guid => ComponentGuid;
    
    public bool PerformAction(BoardController gameBoardController)
    {
		gameBoardController.CreateBoard();
        
        return true;
    }
}