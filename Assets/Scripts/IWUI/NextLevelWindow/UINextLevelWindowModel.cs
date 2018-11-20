using System.Collections.Generic;
using System.Text;

public class UINextLevelWindowModel : IWWindowModel
{
    public string Title => LocalizationService.Instance.Manager.GetTextByUid("window.nexLevel.title", "Next Level!");
    public string Mesage => (GameDataService.Current.LevelsManager.Level + 1).ToString();
    public string Header => LocalizationService.Instance.Manager.GetTextByUid("window.nexLevel.header", "New Recipe!");

    public string Rewards
    {
        get
        {
            var rewards = new StringBuilder();
            var data = GameDataService.Current.LevelsManager.Rewards;
            
            foreach (var pair in data)
            {
                rewards.Append(" ");
                rewards.Append(pair.ToStringIcon());
            }
            
            return string.Format(LocalizationService.Instance.Manager.GetTextByUid("common.message.reward", "Reward:{0}"), rewards);
        }
    }

    public List<OrderDef> Recipes => GameDataService.Current.OrdersManager.Recipes.FindAll(def => def.Level == GameDataService.Current.LevelsManager.Level + 1);
}