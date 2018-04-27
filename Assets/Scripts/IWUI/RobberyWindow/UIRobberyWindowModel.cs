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
}