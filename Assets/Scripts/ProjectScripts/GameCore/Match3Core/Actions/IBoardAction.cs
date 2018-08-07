public interface IBoardAction  
{
	bool PerformAction(BoardController gameBoardController);
	
	int Guid { get;  }
}