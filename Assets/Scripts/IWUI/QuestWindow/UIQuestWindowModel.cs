using System;
using UnityEngine;
using System.Collections.Generic;
using System.Text;
using Quests;

public class UIQuestWindowModel : IWWindowModel
{
    private QuestEntity quest;
    public QuestEntity Quest
    {
        get { return quest; }
        set
        {
            quest = value;
            questDescription = quest?.GetComponent<QuestDescriptionComponent>(QuestDescriptionComponent.ComponentGuid);
            tasks = quest?.Tasks;
        }
    }

    private QuestDescriptionComponent questDescription;

    private List<TaskEntity> tasks;

    private TaskEntity FirstTask => tasks[0];
    
    public string Title => questDescription?.Title;

    public string Message => questDescription?.Message;

    public string Description => FirstTask.GetComponent<QuestDescriptionComponent>(QuestDescriptionComponent.ComponentGuid)?.Message;
    
    public List<CurrencyPair> Reward => FirstTask.GetComponent<QuestRewardComponent>(QuestRewardComponent.ComponentGuid)?.Value;

    public string ButtonText
    {
        get
        {
            switch (Quest.State)
            {
                case TaskState.Pending:
                case TaskState.New:
                case TaskState.InProgress:                                                                                              
                    return "Find";

                case TaskState.Completed:
                    return "Claim";

                case TaskState.Claimed:
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            return string.Empty;
        }
    }

    public string AmountText
    {
        get
        {
            var task = FirstTask;
            TaskCounterEntity counterTask = task as TaskCounterEntity;
            if (counterTask == null)
            {
                return string.Empty;
            }
            
            bool isCompleted = task.IsCompleted();
            int  current     = counterTask.CurrentValue;
            int  target      = counterTask.TargetValue;
                
            return $"<color=#{( isCompleted ? "FFFFFF" : "FE4704")}><size=55>{current}</size></color>/{target}";
        }
    }

    public Dictionary<int, int> ConvertRewardsToDict(List<CurrencyPair> src)
    {
        var rewards = new Dictionary<int, int>();

        foreach (var reward in src)
        {
            var id = PieceType.Parse(reward.Currency);

            if (id == PieceType.None.Id) continue;

            if (rewards.ContainsKey(id))
            {
                rewards[id] += reward.Amount;
                continue;
            }

            rewards.Add(id, reward.Amount);
        }

        return rewards;
    }
    
    public string RewardText
    {
        get
        {
            QuestRewardComponent rewardComponent = quest.GetComponent<QuestRewardComponent>(QuestRewardComponent.ComponentGuid);
            if (rewardComponent == null)
            {
                return string.Empty;
            }

            var rewardDef = rewardComponent.Value;
            
            var types = new List<string>();
            var rewards = new List<string>();
            
            var str = new StringBuilder("<font=\"POETSENONE-REGULAR SDF\" material=\"POETSENONE-REGULAR SDF\"><color=#933E00>Reward:</color></font> <size=50>");
            
            foreach (var reward in rewardDef)
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
                
                var pair = CurrencyHellper.ResourcePieceToCurrence(ConvertRewardsToDict(rewardDef), currency);

                if (pair.Amount == 0) pair.Amount = reward.Amount;
                
                rewards.Add(pair.ToStringIcon(false));
            }
            
            str.Append(string.Join("  ", rewards));
            str.Append("</size>");
            
            return str.ToString();
        }
    }

    public Sprite Icon
    {
        get
        {
            QuestDescriptionComponent cmp = FirstTask.GetComponent<QuestDescriptionComponent>(QuestDescriptionComponent.ComponentGuid);

            if (cmp?.Ico == null)
            {
                return null;
            }
            
            return IconService.Current.GetSpriteById(cmp.Ico);
        }
    }
}
