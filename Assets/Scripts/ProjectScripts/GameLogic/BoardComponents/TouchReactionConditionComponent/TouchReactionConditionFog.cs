public class TouchReactionConditionFog : TouchReactionConditionComponent
{
    public override bool Check(BoardPosition position, Piece piece)
    {
        if (IsDone) return true;
        
        IsDone = GameDataService.Current.HeroesManager.CurrentPower() > 0;
        
        return IsDone;
    }
}