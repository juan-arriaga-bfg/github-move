public class UIMainWindowModel : IWWindowModel 
{
    public string SettingsText { get{ return "SETTINGS"; } }
    public string FightText { get{ return "FIGHT"; } }

    public string GetPriceLabelForFight()
    {
        var currentPrice = GameDataService.Current.EnemiesManager.GetCurrentEnemy().Price.Amount;

        return currentPrice > 0 ? currentPrice.ToString() : "FOR FREE";
    }

    public int GetPriceForFight()
    {
        var currentPrice = GameDataService.Current.EnemiesManager.GetCurrentEnemy().Price.Amount;

        return currentPrice;
    }

}