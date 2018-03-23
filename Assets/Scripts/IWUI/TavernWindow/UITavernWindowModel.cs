using System.Collections.Generic;

public class UITavernWindowModel : IWWindowModel 
{
    public Obstacle Obstacle { get; set; }
    public HeroAbility CurrentAbility { get; set; }
    
    public string Title
    {
        get
        {
            return Obstacle == null
                ? string.Format("Tavern Level {0}", ProfileService.Current.GetStorageItem(Currency.LevelTavern.Name).Amount + 1) : "Heroes";
        }
    }
    
    public string SubTitle
    {
        get { return Obstacle == null ? "Tap to hero for the upgrade" : "Choose hero for the quest:"; }
    }
    
    public string Message
    {
        get
        {
            return Obstacle == null
                ? "Your Heroes"
                : string.Format("{0} Requirements:", CurrentAbility.Ability);
        }
    }
    
    public List<Hero> Heroes()
    {
        return GameDataService.Current.HeroesManager.Heroes;
    }
}
