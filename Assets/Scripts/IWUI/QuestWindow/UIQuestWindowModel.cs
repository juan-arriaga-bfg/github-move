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

    public Dictionary<int, int> PiecesReward;
    public List<CurrencyPair> CurrencysReward;
    
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
    
    public string RewardText
    {
        get
        {
            var str = new StringBuilder("<font=\"POETSENONE-REGULAR SDF\" material=\"POETSENONE-REGULAR SDF\"><color=#933E00>Reward:</color></font> <size=50>");
            
            str.Append(CurrencyHellper.RewardsToString("  ", PiecesReward, CurrencysReward));
            str.Append("</size>");
            
            return str.ToString();
        }
    }

    public Sprite Icon => UiQuestButton.GetIcon(quest);

    public void InitReward()
    {
        var reward = quest.GetComponent<QuestRewardComponent>(QuestRewardComponent.ComponentGuid)?.Value;
        
        if(reward == null) return;

        PiecesReward = CurrencyHellper.FiltrationRewards(reward, out CurrencysReward);
    }
}
