using System.Text;

public class UINextLevelWindowModel : IWWindowModel
{
    public string Title => "Next Level!";
    public string Mesage => (GameDataService.Current.LevelsManager.Level + 1).ToString();

    public string Rewards
    {
        get
        {
            var rewards = new StringBuilder("Rewards:");
            var data = GameDataService.Current.LevelsManager.Rewards;
            
            foreach (var pair in data)
            {
                rewards.Append(" ");
                rewards.Append(pair.ToStringIcon());
            }
            
            return rewards.ToString();
        }
    }
    public string TapText => "Tap to Continue...";
}