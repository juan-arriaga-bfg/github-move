public class TouchReactionConditionHeroLevelUp : TouchReactionConditionComponent
{
    public override bool Check(BoardPosition position, Piece piece)
    {
        if (IsDone) return true;
        
        var hero = GameDataService.Current.GetHero("Robin");
        var level = GameDataService.Current.HeroLevel;
        
        IsDone = ProfileService.Current.GetStorageItem(Currency.RobinCards.Name).Amount >= hero.Prices[level].Amount;
        
        return IsDone;
    }
}