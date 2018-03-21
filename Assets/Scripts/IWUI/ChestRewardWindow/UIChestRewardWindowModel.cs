public class UIChestRewardWindowModel : IWWindowModel
{
    private Chest chest;
    private CurrencyPair reward;
    
    public string Title
    {
        get { return "You Got:"; }
    }
    
    public string Message
    {
        get { return "<color=#FED100>Tap to claim</color>"; }
    }
    
    public string CardHeroText
    {
        get { return "<color=#3D7AA4>Rare card</color>"; }
    }
    
    public string CardBuildText
    {
        get { return "<color=#347E13>Build card</color>"; }
    }
    
    public string CardResourceText
    {
        get { return "<color=#D75100>Resources card</color>"; }
    }

    public Chest Chest
    {
        get { return chest; }
        set
        {
            chest = value;
            reward = null;
        }
    }

    public CurrencyPair GetReward()
    {
        return reward ?? (reward = Chest.GetReward(ProfileService.Current.GetStorageItem(Currency.LevelCastle.Name).Amount));
    }
}