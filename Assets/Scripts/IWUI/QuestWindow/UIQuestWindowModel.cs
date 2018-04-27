using UnityEngine;
using System.Collections;

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
            var amount = 0;

            foreach (var reward in Quest.Rewards)
            {
                var def = GameDataService.Current.PiecesManager.GetPieceDef(reward.Key);
                
                if(def == null) continue;

                amount += def.SpawnResources.Amount * reward.Value;
            }
            
            return string.Format(
                "<font=\"POETSENONE-REGULAR SDF\" material=\"POETSENONE-REGULAR SDF\"><color=#933E00>Reward:</color></font> <size=50>{0} <sprite name={1}></size>",
                amount, Currency.Coins.Name);
        }
    }

    public Sprite Icon
    {
        get { return IconService.Current.GetSpriteById(Quest.WantedIcon); }
    }
}
