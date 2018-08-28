using UnityEngine;
using System.Collections;
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
            var reward = CurrencyHellper.ResourcePieceToCurrence(Quest.Rewards, Currency.Coins.Name);
            
            return $"<font=\"POETSENONE-REGULAR SDF\" material=\"POETSENONE-REGULAR SDF\"><color=#933E00>Reward:</color></font> <size=50>{reward.Amount} <sprite name={reward.Currency}></size>";
        }
    }

    public Sprite Icon => IconService.Current.GetSpriteById(Quest.WantedIcon);
}
