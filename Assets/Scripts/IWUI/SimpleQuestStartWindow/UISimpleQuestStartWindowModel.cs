public class UISimpleQuestStartWindowModel : IWWindowModel 
{
    public Quest Quest { get; set; }
    
    public string Title
    {
        get { return "Sherwood Quest"; }
    }
    
    public string SubTitle
    {
        get { return "Choose party for the quest:"; }
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
