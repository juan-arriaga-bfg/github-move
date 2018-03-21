public class TouchReactonConditionFight : TouchReactionConditionComponent
{
	public override bool Check(BoardPosition position, Piece piece)
	{
		if (IsDone) return true;
		
		var liveComponent = piece.GetComponent<LivePieceComponent>(LivePieceComponent.ComponentGuid);

		if (liveComponent == null) return false;

		var hero = GameDataService.Current.HeroesManager.GetHero("Robin");
		
//		liveComponent.HitPoints -= hero.Def.TimeBonuses[hero.Level];
		
		IsDone = liveComponent.IsLive(position) == false;
        
		return IsDone;
	}
}