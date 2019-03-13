using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class UICharacterUnlockedWindowModel : IWWindowModel
{
    public string CharacterId;

    public List<CurrencyPair> Rewards;
    
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
