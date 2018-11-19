using System.Collections.Generic;

public class AddTutorialAnimation : BaseTutorialAnimation
{
	public int PieceId;
	public BoardPosition Target;
    
	public override void Start()
	{
		if (isStart) return;
        
		base.Start();
		
		if(Hard(Target)) return;

		Find();
	}

	private void Find()
	{
		var positions = new List<BoardPosition>();
		
		if(context.Context.Context.BoardLogic.EmptyCellsFinder.FindRandomNearWithPointInCenter(Target, positions, 1) == false) return;

		Hard(positions[0]);
	}

	private bool Hard(BoardPosition target)
	{
		var board = context.Context.Context;
		
		if(board.BoardLogic.IsEmpty(target) == false) return false;
			
		board.ActionExecutor.AddAction(new SpawnPieceAtAction{At = target, PieceTypeId = PieceId});
		return true;
	}
}