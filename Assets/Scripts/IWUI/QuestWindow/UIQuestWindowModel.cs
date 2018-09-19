using UnityEngine;
using System.Collections.Generic;
using System.Text;

public class UIQuestWindowModel : IWWindowModel
{
    private QuestOld quest;
    public QuestOld Quest
    {
        get { return quest ?? GameDataService.Current.QuestsManagerOld.ActiveQuests[0]; }
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
            var types = new List<string>();
            var rewards = new List<string>();
            
            var str = new StringBuilder("<font=\"POETSENONE-REGULAR SDF\" material=\"POETSENONE-REGULAR SDF\"><color=#933E00>Reward:</color></font> <size=50>");
            
            foreach (var reward in Quest.Def.Rewards)
            {
                var currency = reward.Currency;
                
                if(types.Contains(currency)) continue;
                
                var id = PieceType.Parse(currency);

                if (id != PieceType.None.Id)
                {
                    var def = GameDataService.Current.PiecesManager.GetPieceDef(id);

                    if (def?.SpawnResources == null)
                    {
                        types.Add(currency);
                        rewards.Add(reward.ToStringIcon());
                        continue;
                    }
                    
                    currency = def.SpawnResources.Currency;
                }
                
                if(types.Contains(currency)) continue;
                
                types.Add(currency);
                
                var pair = CurrencyHellper.ResourcePieceToCurrence(Quest.Rewards, currency);

                if (pair.Amount == 0) pair.Amount = reward.Amount;
                
                rewards.Add(pair.ToStringIcon(false));
            }
            
            str.Append(string.Join("  ", rewards));
            str.Append("</size>");
            
            return str.ToString();
        }
    }

    public Sprite Icon => IconService.Current.GetSpriteById(Quest.WantedIcon);
}
