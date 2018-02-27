public class TouchReactonConditionFight : TouchReactionConditionComponent
{
	public override bool Check(BoardPosition position, Piece piece)
	{
		if (IsDone) return true;
		
		var liveComponent = piece.GetComponent<LivePieceComponent>(LivePieceComponent.ComponentGuid);

		if (liveComponent == null) return false;
		
		liveComponent.HitPoints -= 5;
        
		IsDone = liveComponent.IsLive(position) == false;
        
		return IsDone;
	}
}