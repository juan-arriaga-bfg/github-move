using UnityEngine;
using System.Collections.Generic;
using System.Text;

public class UIQuestWindowModel : IWWindowModel
{
    private Quest quest;
    public Quest Quest
    {
        get { return quest ?? GameDataService.Current.QuestsManager.ActiveQuests[0]; }
        set { quest = value; }
    }

    public string Title => $"Quest {Quest.Def.Uid}";

    public string Message => Quest.Def.Message;

    public string Description => "Collect pieces for quest complete";

    public string ButtonText => Quest.Check() ? "Claim" : "Find";

    public string AmountText => $"<color=#{(Quest.Check() ? "FFFFFF" : "FE4704")}><size=55>{Quest.CurrentAmount}</size></color>/{Quest.TargetAmount}";

    public string RewardText
    {
        get
        {
            var types = new List<string>
            {
                Currency.Coins.Name,
                Currency.Crystals.Name,
                Currency.Energy.Name,
                Currency.Mana.Name,
                Currency.Experience.Name,
            };

            var rewards = new List<string>();
            var str = new StringBuilder("<font=\"POETSENONE-REGULAR SDF\" material=\"POETSENONE-REGULAR SDF\"><color=#933E00>Reward:</color></font> <size=50>");
            
            foreach (var type in types)
            {
                var reward = CurrencyHellper.ResourcePieceToCurrence(Quest.Rewards, type);
                
                if(reward.Amount == 0) continue;
                
                rewards.Add($"{reward.Amount} <sprite name={reward.Currency}>");
            }
            
            str.Append(string.Join(", ", rewards));
            str.Append("</size>");
            
            return str.ToString();
        }
    }

    public Sprite Icon => IconService.Current.GetSpriteById(Quest.WantedIcon);
}
