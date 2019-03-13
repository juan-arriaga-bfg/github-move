using System;
using System.Collections.Generic;
using System.Text;

public class UINextLevelWindowModel : IWWindowModel
{
    public string Title => LocalizationService.Get("window.nexLevel.title", "window.nexLevel.title");
    public string Mesage => (GameDataService.Current.LevelsManager.Level + 1).ToString();
    public string Header => LocalizationService.Get("window.nexLevel.header", "window.nexLevel.header");

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
            
            string resultString = string.Format(LocalizationService.Get("common.message.reward", "common.message.reward:{0}"), "");
            resultString = resultString + Environment.NewLine + rewards.Remove(0, 1);
            return resultString;
        }
    }

    public List<OrderDef> Recipes
    {
        get
        {
            var manager = GameDataService.Current.OrdersManager;
            
            return manager.Locker.IsLocked ? new List<OrderDef>() : manager.Recipes.FindAll(def => def.Level == GameDataService.Current.LevelsManager.Level + 1);
        }
    }
}