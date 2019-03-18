using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class UICharacterUnlockedWindowModel : IWWindowModel
{
    public QuestEntity Quest;
    
    // Allow test window without Quest property set
    public bool TestMode;
    
    public string CharacterId
    {
        get
        {
            if (TestMode)
            {
                return UiCharacterData.CharRedHood;
            }
            
            return UiCharacterData.GetDefByPieceId((Quest.Tasks[0] as IHavePieceId).PieceId).Id;
        }
    }

    public List<CurrencyPair> Rewards
    {
        get
        {
            if (TestMode)
            {
                return new List<CurrencyPair>
                {
                    new CurrencyPair {Currency = Currency.Coins.Name, Amount = 123},
                    new CurrencyPair {Currency = Currency.Crystals.Name, Amount = 23}
                };
            }
            
            return Quest.GetComponent<QuestRewardComponent>(QuestRewardComponent.ComponentGuid)?.Value;
        }
    }

    public string RewardsString
    {
        get
        {
            var rewards = new StringBuilder();
            var data = Rewards;
            
            foreach (var pair in data)
            {
                rewards.Append(" ");
                rewards.Append(pair.ToStringIcon());
            }
            
            return string.Format(LocalizationService.Get("common.message.reward", "common.message.reward:{0}"), rewards);
        }
    }

    public override IWWindowModel GetMockModel()
    {
        var model = new UICharacterUnlockedWindowModel();

        return model;
    }
}
