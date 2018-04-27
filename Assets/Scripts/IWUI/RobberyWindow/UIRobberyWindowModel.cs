public class UIRobberyWindowModel : IWWindowModel 
{
    public Enemy Enemy { get; set; }
    public EnemyView View { get; set; }
    
    public string Title
    {
        get { return "Stolen Treasures"; }
    }
    
    public string Message
    {
        get { return "Drag Your Heroes to rob Enemies"; }
    }
    
    public string SendText
    {
        get { return "Send Hero"; }
    }
    
    public string ClaimText
    {
        get { return "Claim Reward"; }
    }

    public Hero Attack()
    {
        var heroes = GameDataService.Current.HeroesManager.Heroes.FindAll(h => h.IsSleep == false);

        if (heroes.Count == 0) return null;
        
        heroes.Shuffle();
        return heroes[0];
    }
}