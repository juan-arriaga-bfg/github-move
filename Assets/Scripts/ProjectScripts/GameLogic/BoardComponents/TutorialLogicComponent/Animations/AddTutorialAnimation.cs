using System.Collections.Generic;

public class AddTutorialAnimation : BaseTutorialAnimation
{
	public int PieceId;
	public List<BoardPosition> Targets;
    
	public override void Start()
	{
		if (isStart) return;
        
		base.Start();
		
		foreach (var target in Targets)
		{
			if(context.Context.Context.BoardLogic.IsEmpty(target) == false) continue;
			
			context.Context.Context.ActionExecutor.AddAction(new SpawnPieceAtAction{At = target, PieceTypeId = PieceId});
			break;
		}
	}
}