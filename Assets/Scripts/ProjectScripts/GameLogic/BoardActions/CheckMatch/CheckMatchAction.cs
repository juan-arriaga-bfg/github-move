using System.Collections.Generic;

public class CheckMatchAction : IBoardAction
{
	public static readonly int ComponentGuid = ECSManager.GetNextGuid();
	
	public virtual int Guid
	{
		get { return ComponentGuid; }
	}
	
	public BoardPosition At { get; set; }

	public List<BoardPosition> MatchField { get; set; }
	
	public bool PerformAction(BoardController gameBoardController)
	{
		MatchField = MatchField ?? new List<BoardPosition>();
		var logic = gameBoardController.BoardLogic;
		
		int currentId;
		
		if (logic.FieldFinder.Find(At, MatchField, out currentId) == false) return false;
		
		var action = logic.MatchActionBuilder.GetMatchAction(MatchField, currentId, At);

		if (action == null) return false;
		
		gameBoardController.ActionExecutor.PerformAction(action);
		
		return true;
	}
}