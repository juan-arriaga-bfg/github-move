using UnityEngine;
using System.Collections;
using System.Text;

public class UIQuestWindowModel : IWWindowModel 
{
    public Quest Quest { get; set; }
    
    public string Title
    {
        get { return string.Format("Quest {0}", Quest.Def.Uid); }
    }

    public string Message
    {
        get { return Quest.Def.Message; }
    }
    
    public string Description
    {
        get { return "Collect pieces for quest complete"; }
    }
    
    public string ButtonText
    {
        get { return Quest.Check() ? "Claim" : "Find"; }
    }
    
    public string AmountText
    {
        get { return string.Format("<color=#{0}><size=55>{1}</size></color>/{2}", Quest.Check() ? "FFFFFF" : "FE4704", Quest.CurrentAmount, Quest.TargetAmount); }
    }

    public string RewardText
    {
        get
        {
            var str = new StringBuilder();

            foreach (var reward in Quest.Def.Rewards)
            {
                str.AppendFormat(
                    "<font=\"POETSENONE-REGULAR SDF\" material=\"POETSENONE-REGULAR SDF\"><color=#933E00>Reward:</color></font> <size=50>{0} <sprite name={1}></size>",
                    reward.Amount, reward.Currency);
            }
            
            return str.ToString();
        }
    }

    public Sprite Icon
    {
        get { return IconService.Current.GetSpriteById(Quest.WantedIcon); }
    }
}
