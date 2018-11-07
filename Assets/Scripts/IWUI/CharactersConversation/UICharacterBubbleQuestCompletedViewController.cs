using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class UICharacterBubbleQuestCompletedViewController : UICharacterBubbleMessageViewController
{
    [SerializeField] private UiQuestButton questButton;
    [SerializeField] private NSText rewardLabel;
    
    public override void Show(UICharacterBubbleDef def, Action onComplete)
    {
        base.Show(def, onComplete);
        
        UiCharacterBubbleDefQuestCompleted data = def as UiCharacterBubbleDefQuestCompleted;

        var quest = GameDataService.Current.QuestsManager.GetActiveQuestById(data.QuestId);
        questButton.Init(quest, false);
        questButton.ToggleCheckMark(true);

        rewardLabel.Text = GetRewardText(quest);
    }

    private string GetRewardText(QuestEntity quest)
    {
        var reward = quest.GetComponent<QuestRewardComponent>(QuestRewardComponent.ComponentGuid)?.Value;

        if (reward == null)
        {
            return "";
        }

        List<CurrencyPair> currencyRewards;
        var priceRewards = CurrencyHellper.FiltrationRewards(reward, out currencyRewards);
        
        var str = new StringBuilder("<font=\"POETSENONE-REGULAR SDF\" material=\"POETSENONE-REGULAR SDF\"><color=#933E00></color></font>");
            
        str.Append(CurrencyHellper.RewardsToString("  ", priceRewards, currencyRewards));
        str.Append("</size>");
            
        return str.ToString();
    }
}