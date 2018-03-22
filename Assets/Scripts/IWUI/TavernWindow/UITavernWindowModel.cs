using System.Collections.Generic;

public class UITavernWindowModel : IWWindowModel 
{
    public Obstacle Obstacle { get; set; }
    
    public string Title
    {
        get { return "Heroes"; }
    }
    
    public string SubTitle
    {
        get { return "Choose hero for the quest:"; }
    }
    
    public string Message
    {
        get { return ""; }
    }
    
    public List<Hero> Heroes()
    {
        return GameDataService.Current.HeroesManager.Heroes;
    }
}
