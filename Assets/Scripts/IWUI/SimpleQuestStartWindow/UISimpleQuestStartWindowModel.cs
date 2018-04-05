public class UISimpleQuestStartWindowModel : IWWindowModel 
{
    public Quest Quest { get; set; }
    
    public string Title
    {
        get { return "Peasant's quest"; }
    }
    
    public string SubTitle
    {
        get { return "Collect pieces for quest complete"; }
    }
    
    public string Message
    {
        get { return Quest.Def.Message; }
    }
    
    public string ButtonText
    {
        get { return "Claim"; }
    }
    
    public string GetChestSkin()
    {
        var chest = GameDataService.Current.ChestsManager.GetChest(Quest.Reward);
        return chest.GetSkin();
    }
}