using System.Collections.Generic;

public class UIHeroesWindowModel : IWWindowModel 
{
    public string Title
    {
        get { return string.Format("Castle Level {0}", ProfileService.Current.GetStorageItem(Currency.LevelCastle.Name).Amount + 1); }
    }
    
    public List<Hero> Heroes
    {
        get { return GameDataService.Current.HeroesManager.Heroes; }
    }
}
